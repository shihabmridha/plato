using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace plato.Views
{
    public partial class ResponseView : UserControl
    {
        public ResponseView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 