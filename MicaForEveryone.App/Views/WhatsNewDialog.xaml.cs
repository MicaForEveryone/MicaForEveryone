using Microsoft.UI.Xaml.Controls;

namespace MicaForEveryone.App.Views
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog(string version)
        {
            this.InitializeComponent();
            VersionTextBlock.Text = $"Version {version} is now installed.";
        }
    }
}
