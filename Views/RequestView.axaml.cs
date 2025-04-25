using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace plato.Views
{
    public partial class RequestView : UserControl
    {
        public RequestView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 