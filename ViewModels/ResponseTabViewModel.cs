using ReactiveUI;

namespace plato.ViewModels;

public class ResponseTabViewModel : ViewModelBase
{
    private string _name;
    private string _content;

    public ResponseTabViewModel()
    {
        _name = string.Empty;
        _content = string.Empty;
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
} 