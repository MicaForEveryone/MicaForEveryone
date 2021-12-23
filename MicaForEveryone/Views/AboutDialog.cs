using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Vanara.PInvoke;

using MicaForEveryone.Extensions;
using MicaForEveryone.UWP;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.Views
{
    public class AboutDialog : XamlDialog
    {
        private static ContentDialogView CreateView() => new ContentDialogView
        {
            ViewModel =
            {
                Title = "Mica For Everyone",
                Content = new TextBlock
                {
                    Inlines =
                    {
                        new Run { Text = "v" + typeof(App).Assembly.GetName().Version },
                        new LineBreak(),
                        new Hyperlink
                        {
                            NavigateUri = new Uri("https://github.com/minusium/MicaForEveryone"),
                            Inlines =
                            {
                                new Run { Text = "GitHub" }
                            }
                        }
                    }
                },
                IsPrimaryButtonEnabled = true,
                PrimaryButtonContent = "Close",
                PrimaryCommand = App.CloseDialogCommand,
            },
        };

        public AboutDialog() : base(CreateView())
        {
            ClassName = nameof(AboutDialog);
            Title = "About";
            Width = 400;
            Height = 350;
            ((ContentDialogView)View).ViewModel.PrimaryCommandParameter = this;
            Create += AboutDialog_Create;
            View.ActualThemeChanged += View_ActualThemeChanged;
        }

        private void SetTitlebarColor(HWND windowHandle)
        {
            var background = ((SolidColorBrush)Windows.UI.Xaml.Application.Current.Resources["ContentDialogBackground"]).Color;
            var topOverlay = ((SolidColorBrush)Windows.UI.Xaml.Application.Current.Resources["ContentDialogTopOverlay"]).Color;
            var bgR = (background.R * (255 - topOverlay.A) + topOverlay.R * topOverlay.A) / 255;
            var bgG = (background.G * (255 - topOverlay.A) + topOverlay.G * topOverlay.A) / 255;
            var bgB = (background.B * (255 - topOverlay.A) + topOverlay.B * topOverlay.A) / 255;

            var foreground = ((SolidColorBrush)Windows.UI.Xaml.Application.Current.Resources["ContentDialogForeground"]).Color;
            windowHandle.SetCaptionColor((byte)bgR, (byte)bgG, (byte)bgB);
            windowHandle.SetCaptionTextColor(foreground.R, foreground.G, foreground.B);
        }

        private void AboutDialog_Create(object sender, Win32.WindowEventArgs e)
        {
            SetTitlebarColor(e.WindowHandle);
        }

        private void View_ActualThemeChanged(object sender, object args)
        {
            SetTitlebarColor(Handle);
        }
    }
}
