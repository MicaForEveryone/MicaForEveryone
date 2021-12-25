using MicaForEveryone.UI;

namespace MicaForEveryone.Views
{
    public class ErrorDialog : ContentDialog
    {
        public ErrorDialog() : base(new ContentDialogView())
        {
            ClassName = nameof(ErrorDialog);
            Title = "Mica For Everyone";
            Width = 576;
            Height = 320;
        }
    }
}
