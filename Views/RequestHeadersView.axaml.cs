using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace plato.Views
{
    public partial class RequestHeadersView : UserControl
    {
        public RequestHeadersView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 