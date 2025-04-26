using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using plato.ViewModels;
using Avalonia.Interactivity;
using System;

namespace plato.Views
{
    public partial class RequestView : UserControl
    {
        private RequestViewModel? ViewModel => DataContext as RequestViewModel;

        public RequestView()
        {
            InitializeComponent();
            
            if (QueryParamsGrid != null)
            {
                QueryParamsGrid.AddHandler(Button.ClickEvent, RemoveQueryParam, handledEventsToo: true);
            }
            
            if (HeadersGrid != null)
            {
                HeadersGrid.AddHandler(Button.ClickEvent, RemoveHeader, handledEventsToo: true);
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void RemoveQueryParam(object? sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && 
                button.CommandParameter is KeyValuePair param && 
                ViewModel != null)
            {
                ViewModel.RemoveQueryParam(param);
                e.Handled = true;
            }
        }
        
        private void RemoveHeader(object? sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && 
                button.CommandParameter is KeyValuePair param && 
                ViewModel != null)
            {
                ViewModel.RemoveHeader(param);
                e.Handled = true;
            }
        }
    }
} 