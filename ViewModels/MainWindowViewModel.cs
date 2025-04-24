namespace plato.ViewModels;

using System.Collections.ObjectModel;

public class MainWindowViewModel : ViewModelBase
{
    private string _url = string.Empty;
    private RequestTabViewModel _selectedRequestTab;
    private ResponseTabViewModel _selectedResponseTab;
    private TreeItemViewModel _selectedTreeItem;

    public MainWindowViewModel()
    {
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
            new ResponseTabViewModel { Name = "Headers" }
        };

        TreeItems = new ObservableCollection<TreeItemViewModel>
        {
            new TreeItemViewModel
            {
                Name = "Collections",
                IsExpanded = true,
                Children = new ObservableCollection<TreeItemViewModel>
                {
                    new TreeItemViewModel { Name = "Collection 1", IsFolder = true },
                    new TreeItemViewModel { Name = "Collection 2", IsFolder = true }
                }
            }
        };

        // Set defaults
        SelectedRequestTab = RequestTabs[0];
        SelectedResponseTab = ResponseTabs[0];
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

    public ObservableCollection<TreeItemViewModel> TreeItems { get; }
    
    public TreeItemViewModel SelectedTreeItem
    {
        get => _selectedTreeItem;
        set => this.RaiseAndSetIfChanged(ref _selectedTreeItem, value);
    }
}
