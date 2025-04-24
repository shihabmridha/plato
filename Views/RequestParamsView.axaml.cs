using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace plato.Views
{
    public partial class RequestParamsView : UserControl
    {
        public RequestParamsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 