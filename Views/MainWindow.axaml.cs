using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using plato.ViewModels;
using System;

namespace plato.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        var requestTabControl = this.FindControl<TabControl>("RequestTabControl");
        var responseTabControl = this.FindControl<TabControl>("ResponseTabControl");
        
        // Wire up selection change events if needed
        if (requestTabControl != null && DataContext is MainWindowViewModel viewModel)
        {
            // Update the selected tab when the tab selection changes
            requestTabControl.SelectionChanged += (s, e) =>
            {
                if (requestTabControl.SelectedIndex >= 0 && requestTabControl.SelectedIndex < viewModel.RequestTabs.Count)
                {
                    viewModel.SelectedRequestTab = viewModel.RequestTabs[requestTabControl.SelectedIndex];
                }
            };
            
            // Update the tab when the ViewModel selection changes
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainWindowViewModel.SelectedRequestTab))
                {
                    var index = viewModel.RequestTabs.IndexOf(viewModel.SelectedRequestTab);
                    if (index >= 0)
                        requestTabControl.SelectedIndex = index;
                }
            };
            
            // Same for response tabs
            if (responseTabControl != null)
            {
                responseTabControl.SelectionChanged += (s, e) =>
                {
                    if (responseTabControl.SelectedIndex >= 0 && responseTabControl.SelectedIndex < viewModel.ResponseTabs.Count)
                    {
                        viewModel.SelectedResponseTab = viewModel.ResponseTabs[responseTabControl.SelectedIndex];
                    }
                };
                
                viewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MainWindowViewModel.SelectedResponseTab))
                    {
                        var index = viewModel.ResponseTabs.IndexOf(viewModel.SelectedResponseTab);
                        if (index >= 0)
                            responseTabControl.SelectedIndex = index;
                    }
                };
            }
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}