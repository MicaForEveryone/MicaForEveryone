using MicaForEveryone.UWP;

namespace MicaForEveryone.Views
{
    public class ErrorDialog : ContentDialog
    {
        private static ContentDialogView CreateView() => new ContentDialogView
        {
            ViewModel =
            {
                Title = "Error",
            },
        };

        public ErrorDialog() : base(CreateView())
        {
            ClassName = "Dialog";
            Title = "Mica For Everyone";
            Width = 576;
            Height = 320;
        }

        public void SetMessage(string message)
        {
            ((ContentDialogView)View).ViewModel.Content = message;
        }
    }
}
