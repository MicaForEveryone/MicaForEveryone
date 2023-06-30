using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MicaForEveryone.Core.Models;

namespace MicaForEveryone.UI.Selectors
{
    internal class BackdropTypeSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate None { get; set; }
        public DataTemplate Mica { get; set; }
        public DataTemplate Acrylic { get; set; }
        public DataTemplate Tabbed { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item as BackdropType?)
            {
                case BackdropType.Default:
                    return Default;
                case BackdropType.None:
                    return None;
                case BackdropType.Mica:
                    return Mica;
                case BackdropType.Acrylic:
                    return Acrylic;
                case BackdropType.Tabbed:
                    return Tabbed;
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
