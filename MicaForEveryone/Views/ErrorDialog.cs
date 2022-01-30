namespace MicaForEveryone.Views
{
    public class ErrorDialog : ContentDialog
    {
        public ErrorDialog() : base(new())
        {
            ClassName = nameof(ErrorDialog);

            var resources = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            Title = resources.GetString("AppName");
        }
    }
}
