using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MicaForEveryone.Models;

namespace MicaForEveryone.UI.Selectors
{
    internal class TitlebarColorSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate System { get; set; }
        public DataTemplate Light { get; set; }
        public DataTemplate Dark { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item as TitlebarColorMode?)
            {
                case TitlebarColorMode.Default:
                    return Default;
                case TitlebarColorMode.System:
                    return System;
                case TitlebarColorMode.Light:
                    return Light;
                case TitlebarColorMode.Dark:
                    return Dark;
                default:
                    return null;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
