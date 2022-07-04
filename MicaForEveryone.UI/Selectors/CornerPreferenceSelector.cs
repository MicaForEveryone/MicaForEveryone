using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MicaForEveryone.Models;

namespace MicaForEveryone.UI.Selectors
{
    internal class CornerPreferenceSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate Square { get; set; }
        public DataTemplate Rounded { get; set; }
        public DataTemplate RoundedSmall { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item as CornerPreference?)
            {
                case CornerPreference.Default:
                    return Default;
                case CornerPreference.Square:
                    return Square;
                case CornerPreference.Rounded:
                    return Rounded;
                case CornerPreference.RoundedSmall:
                    return RoundedSmall;
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
