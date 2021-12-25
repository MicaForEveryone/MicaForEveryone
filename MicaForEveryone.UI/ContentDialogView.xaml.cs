using MicaForEveryone.ViewModels;

namespace MicaForEveryone.UI
{
    public sealed partial class ContentDialogView
    {
        public ContentDialogView()
        {
            InitializeComponent();
        }

        public ContentDialogViewModel ViewModel { get; } = new ContentDialogViewModel();
    }
}
