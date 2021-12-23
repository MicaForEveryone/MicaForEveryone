using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

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
            Height = 300;
            ((ContentDialogView)View).ViewModel.PrimaryCommandParameter = this;
        }
    }
}
