using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MicaForEveryone.Core.Models;

namespace MicaForEveryone.UI.Selectors
{
    internal class CornerPreferenceSelector : DataTemplateSelector
    {
        public DataTemplate Default { get; set; }
        public DataTemplate Square { get; set; }
        public DataTemplate Rounded { get; set; }
        public DataTemplate RoundedSmall { get; set; }

        protected override DataTemplate SelectTemplateCore(object item) =>
            item switch
            {
                CornerPreference.Default => Default,
                CornerPreference.Square => Square,
                CornerPreference.Rounded => Rounded,
                CornerPreference.RoundedSmall => RoundedSmall,
                _ => null
            };

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
