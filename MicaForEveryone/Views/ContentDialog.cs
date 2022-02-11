using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using MicaForEveryone.UI;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Views
{
    public class ContentDialog : XamlDialog
    {
        protected static ICommand CloseDialogCommand { get; } =
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

        public ContentDialog() : this(new())
        {
        }

        protected ContentDialog(ContentDialogView view) : 
            this(view, Program.CurrentApp.Container.GetService<IContentDialogViewModel>())
        {
            ViewModel.IsPrimaryButtonEnabled = true;
            ViewModel.PrimaryCommand = CloseDialogCommand;
            ViewModel.PrimaryCommandParameter = this;

            var resources = ResourceLoader.GetForCurrentView();
            ViewModel.PrimaryButtonContent = resources.GetString("OkButton/Text");
        }

        protected ContentDialog(ContentDialogView view, IContentDialogViewModel viewModel) : base(view)
        {
            ViewModel = viewModel;

            _view = view;
            _view.ViewModel = ViewModel;
            _view.ActualThemeChanged += View_ActualThemeChanged;
        }

        public IContentDialogViewModel ViewModel { get; }

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
            var background = GetResourceColor(resources, "ContentDialogBackground");
            var topOverlay = GetResourceColor(resources, "ContentDialogTopOverlay");
            var captionBackground = background.Blend(topOverlay);
            captionBackground.A = 0;

            var foreground = GetResourceColor(resources, "ContentDialogForeground");
            var captionTextColor = captionBackground.Blend(foreground);
            captionTextColor.A = 0;

            DesktopWindowManager.SetCaptionColor(Handle, captionBackground);
            DesktopWindowManager.SetCaptionTextColor(Handle, captionTextColor);
        }
    }
}
