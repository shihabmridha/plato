using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using plato.Models;
using plato.Services;
using ReactiveUI;

namespace plato.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICollectionService _collectionService;
    private string _url = string.Empty;
    private RequestTabViewModel _selectedRequestTab;
    private ResponseTabViewModel _selectedResponseTab;
    private TreeItemViewModel _selectedTreeItem;
    private ObservableCollection<RequestViewModel> _openRequests = new ObservableCollection<RequestViewModel>();
    private RequestViewModel _activeRequest;
    private ObservableCollection<CollectionModel> _collections = new ObservableCollection<CollectionModel>();
    private bool _isLoadingCollections;

    public MainWindowViewModel(IHttpClientService httpClientService, ICollectionService collectionService)
    {
        _httpClientService = httpClientService;
        _collectionService = collectionService;
        
        // Initialize collections
        RequestTabs = new ObservableCollection<RequestTabViewModel>
        {
            new RequestTabViewModel { Name = "Params" },
            new RequestTabViewModel { Name = "Body" },
            new RequestTabViewModel { Name = "Headers" },
            new RequestTabViewModel { Name = "Auth" }
        };

        ResponseTabs = new ObservableCollection<ResponseTabViewModel>
        {
            new ResponseTabViewModel { Name = "Response" },
            new ResponseTabViewModel { Name = "Headers" },
            new ResponseTabViewModel { Name = "Timeline" }
        };

        // Initialize commands
        NewRequestCommand = ReactiveCommand.Create(NewRequest);
        CloseRequestCommand = ReactiveCommand.Create<RequestViewModel>(CloseRequest);
        
        // Initialize with a default request
        NewRequest();
        
        // Set defaults
        SelectedRequestTab = RequestTabs[0];
        SelectedResponseTab = ResponseTabs[0];
        
        // Load collections
        LoadCollectionsAsync().FireAndForget();
    }

    private TreeItemViewModel CreateSampleCollection(string name)
    {
        return new TreeItemViewModel
        {
            Name = name,
            IsFolder = true,
            Children = new ObservableCollection<TreeItemViewModel>
            {
                new TreeItemViewModel
                {
                    Name = "Authentication",
                    IsFolder = true,
                    Children = new ObservableCollection<TreeItemViewModel>
                    {
                        new TreeItemViewModel { Name = "Login", IsFolder = false },
                        new TreeItemViewModel { Name = "Register", IsFolder = false },
                        new TreeItemViewModel { Name = "Refresh Token", IsFolder = false }
                    }
                },
                new TreeItemViewModel
                {
                    Name = "Users",
                    IsFolder = true,
                    Children = new ObservableCollection<TreeItemViewModel>
                    {
                        new TreeItemViewModel { Name = "Get User Profile", IsFolder = false },
                        new TreeItemViewModel { Name = "Update User", IsFolder = false },
                        new TreeItemViewModel { Name = "Delete User", IsFolder = false }
                    }
                },
                new TreeItemViewModel { Name = "Health Check", IsFolder = false }
            }
        };
    }

    public string Url
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public ObservableCollection<RequestTabViewModel> RequestTabs { get; }
    
    public RequestTabViewModel SelectedRequestTab
    {
        get => _selectedRequestTab;
        set => this.RaiseAndSetIfChanged(ref _selectedRequestTab, value);
    }

    public ObservableCollection<ResponseTabViewModel> ResponseTabs { get; }
    
    public ResponseTabViewModel SelectedResponseTab
    {
        get => _selectedResponseTab;
        set => this.RaiseAndSetIfChanged(ref _selectedResponseTab, value);
    }

    public ObservableCollection<TreeItemViewModel> TreeItems { get; } = new ObservableCollection<TreeItemViewModel>();
    
    public TreeItemViewModel SelectedTreeItem
    {
        get => _selectedTreeItem;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTreeItem, value);
            
            // If a non-folder item is selected in the tree
            if (value != null && !value.IsFolder)
            {
                // In a real implementation, we would load the request from the collection
                // For now, just create a new request
                // LoadRequestFromCollection(value.Id);
            }
        }
    }
    
    public ObservableCollection<RequestViewModel> OpenRequests
    {
        get => _openRequests;
        set => this.RaiseAndSetIfChanged(ref _openRequests, value);
    }
    
    public RequestViewModel ActiveRequest
    {
        get => _activeRequest;
        set => this.RaiseAndSetIfChanged(ref _activeRequest, value);
    }
    
    public ObservableCollection<CollectionModel> Collections
    {
        get => _collections;
        set => this.RaiseAndSetIfChanged(ref _collections, value);
    }
    
    public bool IsLoadingCollections
    {
        get => _isLoadingCollections;
        set => this.RaiseAndSetIfChanged(ref _isLoadingCollections, value);
    }
    
    // Commands
    public ReactiveCommand<Unit, Unit> NewRequestCommand { get; }
    public ReactiveCommand<RequestViewModel, Unit> CloseRequestCommand { get; }
    
    // Methods
    private void NewRequest()
    {
        var request = new RequestViewModel(_httpClientService, _collectionService)
        {
            Url = "https://jsonplaceholder.typicode.com/todos/1" // Default example URL
        };
        
        OpenRequests.Add(request);
        ActiveRequest = request;
    }
    
    private void CloseRequest(RequestViewModel request)
    {
        if (OpenRequests.Contains(request))
        {
            // If we're closing the active request, set a new active request
            if (ActiveRequest == request)
            {
                var index = OpenRequests.IndexOf(request);
                if (index > 0)
                {
                    ActiveRequest = OpenRequests[index - 1];
                }
                else if (OpenRequests.Count > 1)
                {
                    ActiveRequest = OpenRequests[1];
                }
                else
                {
                    // If this was the last request, create a new one
                    OpenRequests.Remove(request);
                    NewRequest();
                    return;
                }
            }
            
            OpenRequests.Remove(request);
        }
    }
    
    private async Task LoadCollectionsAsync()
    {
        try
        {
            IsLoadingCollections = true;
            
            // Load collections from service
            var collections = await _collectionService.GetCollectionsAsync();
            Collections.Clear();
            
            // Add each collection to our observable collection
            foreach (var collection in collections)
            {
                Collections.Add(collection);
            }
            
            // Update the tree items
            UpdateCollectionsTree();
        }
        catch (Exception ex)
        {
            // Handle error
            Console.WriteLine($"Error loading collections: {ex.Message}");
        }
        finally
        {
            IsLoadingCollections = false;
        }
    }
    
    private void UpdateCollectionsTree()
    {
        TreeItems.Clear();
        
        // Create the root collections folder
        var collectionsRoot = new TreeItemViewModel
        {
            Name = "Collections",
            IsFolder = true,
            IsExpanded = true,
            Children = new ObservableCollection<TreeItemViewModel>()
        };
        
        // Add each collection
        foreach (var collection in Collections)
        {
            var collectionItem = new TreeItemViewModel
            {
                Name = collection.Name,
                IsFolder = true,
                ItemId = collection.Id.ToString(),
                Children = new ObservableCollection<TreeItemViewModel>()
            };
            
            // Add collection items recursively
            AddCollectionItemsToTree(collectionItem.Children, collection.Items);
            
            collectionsRoot.Children.Add(collectionItem);
        }
        
        TreeItems.Add(collectionsRoot);
    }
    
    private void AddCollectionItemsToTree(ObservableCollection<TreeItemViewModel> treeItems, ObservableCollection<CollectionItemModel> collectionItems)
    {
        foreach (var item in collectionItems)
        {
            var treeItem = new TreeItemViewModel
            {
                Name = item.Name,
                IsFolder = item.Type == CollectionItemType.Folder,
                ItemId = item.Id.ToString(),
                Children = new ObservableCollection<TreeItemViewModel>()
            };
            
            if (item.Type == CollectionItemType.Folder)
            {
                AddCollectionItemsToTree(treeItem.Children, item.Children);
            }
            
            treeItems.Add(treeItem);
        }
    }
}
