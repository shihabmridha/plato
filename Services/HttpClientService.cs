using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using plato.Models;

namespace plato.Services
{
    public class HttpClientService : IHttpClientService, IDisposable
    {
        private readonly HttpClient _httpClient;
        
        public HttpClientService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseModel> SendRequestAsync(HttpRequestModel request)
        {
            var response = new HttpResponseModel();
            var sw = new Stopwatch();
            
            try
            {
                var requestMessage = CreateRequestMessage(request);
                
                sw.Start();
                var httpResponse = await _httpClient.SendAsync(requestMessage);
                sw.Stop();

                response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
                response.ResponseTime = DateTime.Now;
                response.StatusCode = httpResponse.StatusCode;
                response.ReasonPhrase = httpResponse.ReasonPhrase;
                response.IsSuccess = httpResponse.IsSuccessStatusCode;
                
                // Get headers
                foreach (var header in httpResponse.Headers)
                {
                    response.Headers[header.Key] = string.Join(", ", header.Value);
                }
                
                if (httpResponse.Content != null)
                {
                    // Get content headers
                    foreach (var header in httpResponse.Content.Headers)
                    {
                        response.Headers[header.Key] = string.Join(", ", header.Value);
                    }
                    
                    // Set content type
                    if (httpResponse.Content.Headers.ContentType != null)
                    {
                        response.ContentType = httpResponse.Content.Headers.ContentType.ToString();
                    }
                    
                    // Get content
                    response.Body = await httpResponse.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
                response.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            }
            
            return response;
        }

        public async Task<string> FormatResponseAsync(HttpResponseModel response)
        {
            if (string.IsNullOrEmpty(response.Body))
            {
                return string.Empty;
            }

            // Check if it's JSON
            if (response.ContentType.Contains("application/json") || 
                TryParseJson(response.Body))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(response.Body);
                    return JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                }
                catch
                {
                    // If parsing fails, just return the original body
                    return response.Body;
                }
            }

            // If it's not JSON, just return the body as is
            return response.Body;
        }

        public string BuildUrlWithQueryParameters(string baseUrl, Dictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return baseUrl;
            }

            var uriBuilder = new UriBuilder(baseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var param in parameters)
            {
                query[param.Key] = param.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri.ToString();
        }

        private HttpRequestMessage CreateRequestMessage(HttpRequestModel request)
        {
            // Build URL with query parameters
            var url = request.Url;
            if (request.QueryParameters.Count > 0)
            {
                url = BuildUrlWithQueryParameters(url, request.QueryParameters);
            }
            
            // Create request message
            var requestMessage = new HttpRequestMessage(request.Method, url);
            
            // Add headers
            foreach (var header in request.Headers)
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            
            // Add authentication
            switch (request.AuthType)
            {
                case AuthenticationType.Basic:
                    if (request.AuthParameters.TryGetValue("username", out var username) && 
                        request.AuthParameters.TryGetValue("password", out var password))
                    {
                        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
                    }
                    break;
                
                case AuthenticationType.Bearer:
                    if (request.AuthParameters.TryGetValue("token", out var token))
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    break;
                
                case AuthenticationType.ApiKey:
                    if (request.AuthParameters.TryGetValue("key", out var key) && 
                        request.AuthParameters.TryGetValue("value", out var value) &&
                        request.AuthParameters.TryGetValue("in", out var location))
                    {
                        if (location.Equals("header", StringComparison.OrdinalIgnoreCase))
                        {
                            requestMessage.Headers.TryAddWithoutValidation(key, value);
                        }
                        else if (location.Equals("query", StringComparison.OrdinalIgnoreCase))
                        {
                            var uriBuilder = new UriBuilder(requestMessage.RequestUri);
                            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                            query[key] = value;
                            uriBuilder.Query = query.ToString();
                            requestMessage.RequestUri = uriBuilder.Uri;
                        }
                    }
                    break;
                
                case AuthenticationType.OAuth2:
                    // OAuth2 implementation would depend on the flow chosen
                    // This is a simplified version just adding the access token
                    if (request.AuthParameters.TryGetValue("access_token", out var accessToken))
                    {
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    }
                    break;
            }
            
            // Add body
            if (request.Method != HttpMethod.Get && !string.IsNullOrEmpty(request.Body))
            {
                requestMessage.Content = new StringContent(request.Body, Encoding.UTF8, request.ContentType);
            }
            
            return requestMessage;
        }

        private bool TryParseJson(string text)
        {
            try
            {
                JsonDocument.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
} 