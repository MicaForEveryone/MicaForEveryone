using MicaForEveryone.ViewModels;

namespace MicaForEveryone.UWP
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
