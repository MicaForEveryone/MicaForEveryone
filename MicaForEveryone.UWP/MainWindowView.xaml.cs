using Windows.UI.Xaml;

using MicaForEveryone.ViewModels;
using MicaForEveryone.Models;

namespace MicaForEveryone.UWP
{
    public sealed partial class MainWindowView
    {
        public static Visibility VisibleOrCollapsed(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        public MainWindowView()
        {
            InitializeComponent();
        }

        public MainViewModel ViewModel { get; } = new MainViewModel();
    }
}
