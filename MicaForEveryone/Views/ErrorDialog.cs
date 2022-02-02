using Windows.ApplicationModel.Resources;

namespace MicaForEveryone.Views
{
    public class ErrorDialog : ContentDialog
    {
        public ErrorDialog() : base(new())
        {
            ClassName = nameof(ErrorDialog);

            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("AppName");
        }
    }
}
