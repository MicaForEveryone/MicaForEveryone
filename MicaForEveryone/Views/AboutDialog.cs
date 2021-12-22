using System;
using System.Collections.Generic;
using System.Text;
using MicaForEveryone.UWP;
using MicaForEveryone.Xaml;
using Windows.UI.Xaml.Controls;

namespace MicaForEveryone.Views
{
    public class AboutDialog : XamlDialog
    {
        private static ContentDialogView CreateView() => new ContentDialogView
        {
            ViewModel =
            {
                Title = "Mica For Everyone",
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = "v" + typeof(App).Assembly.GetName().Version},
                        new HyperlinkButton
                        {
                            Content = "Github",
                            Command = App.UrlLauncherCommand,
                            CommandParameter = new Uri("https://github.com/minusium/MicaForEveryone"),
                        },
                    },
                    Spacing = 5,
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
            Height = 600;
            ((ContentDialogView)View).ViewModel.PrimaryCommandParameter = this;
        }
    }
}
