using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        public ISettingsViewModel ViewModel { get; set; }
    }
}
