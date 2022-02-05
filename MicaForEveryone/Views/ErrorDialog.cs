using Windows.ApplicationModel.Resources;

namespace MicaForEveryone.Views
{
    public class ErrorDialog : ContentDialog
    {
        public ErrorDialog() : base(new())
        {
            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("AppName");
        }
    }
}
