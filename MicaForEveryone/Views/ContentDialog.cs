using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using MicaForEveryone.Extensions;
using MicaForEveryone.UWP;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.Views
{
    public class ContentDialog : XamlDialog
    {
        protected ContentDialog(ContentDialogView view) : base(view)
        {
            view.ViewModel.IsPrimaryButtonEnabled = true;
            view.ViewModel.PrimaryButtonContent = "OK";
            view.ViewModel.PrimaryCommand = App.CloseDialogCommand;
            view.ViewModel.PrimaryCommandParameter = this;
            view.ActualThemeChanged += View_ActualThemeChanged;
        }

        public override void Activate()
        {
            base.Activate();
            SetTitlebarColor();
        }

        private void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            SetTitlebarColor();
        }

        private void SetTitlebarColor()
        {
            var resources = Windows.UI.Xaml.Application.Current.Resources;
            var background = ((SolidColorBrush)resources["ContentDialogBackground"]).Color;
            var topOverlay = ((SolidColorBrush)resources["ContentDialogTopOverlay"]).Color;
            var foreground = ((SolidColorBrush)resources["ContentDialogForeground"]).Color;
            background.Blend(topOverlay);

            Handle.SetCaptionColor(background.R, background.G, background.B);
            Handle.SetCaptionTextColor(foreground.R, foreground.G, foreground.B);
        }
    }
}
