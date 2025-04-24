using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace plato.Views
{
    public partial class ResponseHeadersView : UserControl
    {
        public ResponseHeadersView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 