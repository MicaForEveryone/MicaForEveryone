using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class ContentDialogView
    {
        public ContentDialogView()
        {
            InitializeComponent();
        }

        public IContentDialogViewModel ViewModel { get; set; }
    }
}
