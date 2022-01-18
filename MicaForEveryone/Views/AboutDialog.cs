using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

using MicaForEveryone.UI;

namespace MicaForEveryone.Views
{
    public class AboutDialog : ContentDialog
    {
        public AboutDialog() : base(new())
        {
            ClassName = nameof(AboutDialog);
            Title = "About";
            Width = 320;
            Height = 275;

            SetTitle("Mica For Everyone!");

            var content = new TextBlock
            {
                Inlines =
                {
                    new Run
                    {
                        Text = "v" + typeof(App).Assembly.GetName().Version
                    },
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
            };
            SetContent(content);
        }
    }
}
