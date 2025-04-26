using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using plato.Models;
using plato.Services;
using ReactiveUI;

namespace plato.ViewModels
{
    public class RequestViewModel : ViewModelBase
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ICollectionService _collectionService;
        
        private string _url = string.Empty;
        private string _requestBody = string.Empty;
        private string _responseBody = string.Empty;
        private string _requestBodyContentType = "application/json";
        private HttpRequestModel _currentRequest;
        private HttpResponseModel _currentResponse;
        private string _selectedHttpMethod = "GET";
        private bool _isSending;
        private string _statusMessage = string.Empty;
        private string _statusCode = string.Empty;
        private long _responseTime;
        private bool _showResponse;
        
        public RequestViewModel(IHttpClientService httpClientService, ICollectionService collectionService)
        {
            _httpClientService = httpClientService;
            _collectionService = collectionService;
            
            _currentRequest = new HttpRequestModel();
            
            // Initialize collections
            QueryParams = new ObservableCollection<KeyValuePair>();
            Headers = new ObservableCollection<KeyValuePair>();
            
            // Initialize commands
            SendRequestCommand = ReactiveCommand.CreateFromTask(SendRequestAsync);
            SaveToCollectionCommand = ReactiveCommand.CreateFromTask<string>(SaveToCollectionAsync);
            AddQueryParamCommand = ReactiveCommand.Create(AddQueryParam);
            RemoveQueryParamCommand = ReactiveCommand.Create<KeyValuePair>(RemoveQueryParam);
            AddHeaderCommand = ReactiveCommand.Create(AddHeader);
            RemoveHeaderCommand = ReactiveCommand.Create<KeyValuePair>(RemoveHeader);
            
            // Available HTTP methods
            HttpMethods = new ObservableCollection<string> { "GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS" };
            
            // Add common content types
            ContentTypes = new ObservableCollection<string>
            {
                "application/json",
                "application/xml",
                "text/plain",
                "text/html",
                "application/x-www-form-urlencoded",
                "multipart/form-data"
            };
        }

        public string Url
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }

        public string RequestBody
        {
            get => _requestBody;
            set => this.RaiseAndSetIfChanged(ref _requestBody, value);
        }

        public string ResponseBody
        {
            get => _responseBody;
            set => this.RaiseAndSetIfChanged(ref _responseBody, value);
        }

        public string RequestBodyContentType
        {
            get => _requestBodyContentType;
            set => this.RaiseAndSetIfChanged(ref _requestBodyContentType, value);
        }

        public string SelectedHttpMethod
        {
            get => _selectedHttpMethod;
            set => this.RaiseAndSetIfChanged(ref _selectedHttpMethod, value);
        }

        public bool IsSending
        {
            get => _isSending;
            set => this.RaiseAndSetIfChanged(ref _isSending, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public string StatusCode
        {
            get => _statusCode;
            set => this.RaiseAndSetIfChanged(ref _statusCode, value);
        }

        public long ResponseTime
        {
            get => _responseTime;
            set => this.RaiseAndSetIfChanged(ref _responseTime, value);
        }

        public bool ShowResponse
        {
            get => _showResponse;
            set => this.RaiseAndSetIfChanged(ref _showResponse, value);
        }

        // Collections
        public ObservableCollection<KeyValuePair> QueryParams { get; }
        public ObservableCollection<KeyValuePair> Headers { get; }
        public ObservableCollection<string> HttpMethods { get; }
        public ObservableCollection<string> ContentTypes { get; }

        // Commands
        public ReactiveCommand<Unit, Unit> SendRequestCommand { get; }
        public ReactiveCommand<string, Unit> SaveToCollectionCommand { get; }
        public ReactiveCommand<Unit, Unit> AddQueryParamCommand { get; }
        public ReactiveCommand<KeyValuePair, Unit> RemoveQueryParamCommand { get; }
        public ReactiveCommand<Unit, Unit> AddHeaderCommand { get; }
        public ReactiveCommand<KeyValuePair, Unit> RemoveHeaderCommand { get; }

        // Methods
        private async Task SendRequestAsync()
        {
            if (string.IsNullOrEmpty(Url))
            {
                StatusMessage = "URL cannot be empty";
                return;
            }

            try
            {
                IsSending = true;
                StatusMessage = "Sending request...";
                ShowResponse = false;

                // Create request model
                var request = new HttpRequestModel
                {
                    Url = Url,
                    Method = new HttpMethod(SelectedHttpMethod),
                    Body = RequestBody,
                    ContentType = RequestBodyContentType,
                    LastExecutedAt = DateTime.Now
                };

                // Add query parameters
                foreach (var param in QueryParams.Where(p => p.IsEnabled))
                {
                    request.QueryParameters[param.Key] = param.Value;
                }

                // Add headers
                foreach (var header in Headers.Where(h => h.IsEnabled))
                {
                    request.Headers[header.Key] = header.Value;
                }

                _currentRequest = request;
                
                // Send request
                var response = await _httpClientService.SendRequestAsync(request);
                _currentResponse = response;

                // Update UI
                StatusCode = ((int)response.StatusCode).ToString();
                StatusMessage = response.ReasonPhrase;
                ResponseTime = response.ElapsedMilliseconds;
                ResponseBody = await _httpClientService.FormatResponseAsync(response);

                ShowResponse = true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsSending = false;
            }
        }

        private async Task SaveToCollectionAsync(string collectionId)
        {
            if (_currentRequest == null)
            {
                StatusMessage = "No request to save";
                return;
            }

            try
            {
                var success = await _collectionService.SaveRequestToCollectionAsync(
                    Guid.Parse(collectionId),
                    _currentRequest,
                    $"{SelectedHttpMethod} {new Uri(Url).AbsolutePath}"
                );

                StatusMessage = success ? "Request saved to collection" : "Failed to save request";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        private void AddQueryParam()
        {
            QueryParams.Add(new KeyValuePair { Key = "", Value = "", IsEnabled = true });
        }

        public void RemoveQueryParam(KeyValuePair param)
        {
            QueryParams.Remove(param);
        }

        private void AddHeader()
        {
            Headers.Add(new KeyValuePair { Key = "", Value = "", IsEnabled = true });
        }

        public void RemoveHeader(KeyValuePair header)
        {
            Headers.Remove(header);
        }
    }

    public class KeyValuePair : ViewModelBase
    {
        private string _key = string.Empty;
        private string _value = string.Empty;
        private bool _isEnabled = true;

        public string Key
        {
            get => _key;
            set => this.RaiseAndSetIfChanged(ref _key, value);
        }

        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }
    }
} 