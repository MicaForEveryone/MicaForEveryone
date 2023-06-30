using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MicaForEveryone.Core.Models;

namespace MicaForEveryone.UI.Selectors
{
    internal class TitlebarColorSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate System { get; set; }
        public DataTemplate Light { get; set; }
        public DataTemplate Dark { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) =>
            item switch
            {
                TitlebarColorMode.Default => Default,
                TitlebarColorMode.System => System,
                TitlebarColorMode.Light => Light,
                TitlebarColorMode.Dark => Dark,
                _ => null
            };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
