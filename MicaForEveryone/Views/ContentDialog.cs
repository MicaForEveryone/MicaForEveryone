using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using MicaForEveryone.Extensions;
using MicaForEveryone.UI;
using MicaForEveryone.Xaml;
using MicaForEveryone.Win32;

namespace MicaForEveryone.Views
{
    public class ContentDialog : XamlDialog
    {
        private readonly ContentDialogView _view;

        protected ContentDialog(ContentDialogView view) : base(view)
        {
            _view = view;
            _view.ViewModel.IsPrimaryButtonEnabled = true;
            _view.ViewModel.PrimaryButtonContent = "OK";
            _view.ViewModel.PrimaryCommand = App.CloseDialogCommand;
            _view.ViewModel.PrimaryCommandParameter = this;
            _view.ActualThemeChanged += View_ActualThemeChanged;
        }

        public override void Activate()
        {
            base.Activate();
            SetTitlebarColor();
        }

        public void SetTitle(object title)
        {
            _view.ViewModel.Title = title;
        }

        public void SetContent(object content)
        {
            _view.ViewModel.Content = content;
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
