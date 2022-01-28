using MicaForEveryone.ViewModels;
using Windows.UI.Xaml.Controls;

namespace MicaForEveryone.UI
{
    public sealed partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public ISettingsViewModel ViewModel { get; set; }
    }
}
