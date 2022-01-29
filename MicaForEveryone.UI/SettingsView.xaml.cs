using MicaForEveryone.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            #if DEBUG
            rootElement.Background = new Brushes.BackdropBrushXaml();
            #endif
        }

        public ISettingsViewModel ViewModel { get; set; }
    }
}
