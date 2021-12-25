using Windows.UI.Xaml;

using MicaForEveryone.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class TrayIconView
    {
        public TrayIconView()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel { get; } = new MainViewModel();
    }
}
