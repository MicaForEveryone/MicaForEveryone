using System;
using Windows.System;
using Windows.UI.Xaml.Controls;

using static Vanara.PInvoke.User32;

using MicaForEveryone.UWP;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Xaml;

namespace MicaForEveryone
{
    internal partial class App
    {
        private void ShowAboutDialog()
        {
            var openUrlCommand = new RelyCommand(async url =>
                await Launcher.LaunchUriAsync((Uri)url));
            var view = new ContentDialogView
            {
                ViewModel =
                {
                    Title = "Mica For Everyone",
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock { Text = "v" + typeof(App).Assembly.GetName().Version},
                            new HyperlinkButton {
                                Content = "Github",
                                Command = openUrlCommand,
                                CommandParameter = new Uri("https://github.com/minusium/MicaForEveryone"),
                            },
                        },
                        Spacing = 5,
                    },
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonContent = "Close",
                },
            };
            var xamlDialog = new XamlDialog(view)
            {
                ClassName = "Dialog",
                Parent = _mainWindow.Handle,
                Title = "About",
                Width = 400,
                Height = 600,
            };
            view.ViewModel.PrimaryCommand = new RelyCommand(_ =>
            {
                xamlDialog.Close();
                xamlDialog.Dispose();
            });
            xamlDialog.CenterToDesktop();
            xamlDialog.Activate();
            xamlDialog.Show();
            SetForegroundWindow(xamlDialog.Handle);
        }

        private void ShowWindows11RequiredDialog()
        {
            var errorContent = new ContentDialogView
            {
                ViewModel = 
                {
                    Title = "Error",
                    Content = "Mica for Everyone at least requires Windows 11 (10.0.22000) to work.",
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonContent = "OK",
                },
            };
            using var errorDialog = new XamlDialog(errorContent)
            {
                ClassName = "Dialog",
                Title = "Mica For Everyone",
                Width = 576,
                Height = 320,
            };
            errorContent.ViewModel.PrimaryCommand = new RelyCommand(_ =>
            {
                errorDialog.Close();
                Exit();
            });
            errorDialog.CenterToDesktop();
            errorDialog.Activate();
            errorDialog.Show();
            Run(errorDialog);
        }
    }
}