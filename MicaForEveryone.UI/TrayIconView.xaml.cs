using MicaForEveryone.Core.Ui.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class TrayIconView
    {
        public TrayIconView()
        {
            InitializeComponent();
        }

        public ITrayIconViewModel ViewModel { get; set; }
    }
}
