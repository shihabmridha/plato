namespace plato.ViewModels;

using System.Collections.ObjectModel;

public class TreeItemViewModel : ViewModelBase
{
    private string _name;
    private bool _isExpanded;
    private bool _isSelected;
    private bool _isFolder;
    private string _itemId;
    private ObservableCollection<TreeItemViewModel> _children;

    public TreeItemViewModel()
    {
        _name = string.Empty;
        _itemId = string.Empty;
        _children = new ObservableCollection<TreeItemViewModel>();
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }

    public bool IsFolder
    {
        get => _isFolder;
        set => this.RaiseAndSetIfChanged(ref _isFolder, value);
    }
    
    public string ItemId
    {
        get => _itemId;
        set => this.RaiseAndSetIfChanged(ref _itemId, value);
    }

    public ObservableCollection<TreeItemViewModel> Children
    {
        get => _children;
        set => this.RaiseAndSetIfChanged(ref _children, value);
    }
} 