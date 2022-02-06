using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.UI;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;
using MicaForEveryone.Win32.PInvoke;
using System;

namespace MicaForEveryone.Views
{
    public class ContentDialog : XamlDialog
    {
        private static RelyCommand CloseDialogCommand { get; } =
            new RelyCommand(dialog => ((Dialog)dialog).Close());

        private static COLORREF GetResourceColor(ResourceDictionary resources, string name)
        {
            var result = ((SolidColorBrush)resources[name]).Color;
            return new COLORREF
            {
                A = (byte)(255 - result.A),
                R = result.R,
                G = result.G,
                B = result.B,
            };
        }

        private readonly ContentDialogView _view;

        protected ContentDialog(ContentDialogView view) : base(view)
        {
            _view = view;
            _view.ViewModel = Program.CurrentApp.Container.GetService<IContentDialogViewModel>();
            _view.ViewModel.IsPrimaryButtonEnabled = true;
            _view.ViewModel.PrimaryCommand = CloseDialogCommand;
            _view.ViewModel.PrimaryCommandParameter = this;

            var resources = ResourceLoader.GetForCurrentView();
            _view.ViewModel.PrimaryButtonContent = resources.GetString("OkButton/Text");

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
            var background = GetResourceColor(resources, "ContentDialogBackground");
            var topOverlay = GetResourceColor(resources, "ContentDialogTopOverlay");
            var captionBackground = background.Blend(topOverlay);
            captionBackground.A = 0;

            var foreground = GetResourceColor(resources, "ContentDialogForeground");

            DesktopWindowManager.SetCaptionColor(Handle, captionBackground);
            DesktopWindowManager.SetCaptionTextColor(Handle, foreground);
        }
    }
}
