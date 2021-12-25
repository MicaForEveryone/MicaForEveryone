using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

using MicaForEveryone.UI;

namespace MicaForEveryone.Views
{
    public class AboutDialog : ContentDialog
    {
        private static ContentDialogView CreateView() => new ContentDialogView
        {
            ViewModel =
            {
                Title = "Mica For Everyone!",
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
            },
        };

        public AboutDialog() : base(CreateView())
        {
            ClassName = nameof(AboutDialog);
            Title = "About";
            Width = 320;
            Height = 275;
        }
    }
}
