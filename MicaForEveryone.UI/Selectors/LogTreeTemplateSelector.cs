using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.UI.Selectors
{
    internal class LogTreeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LogEntryTemplate { get; set; }
        public DataTemplate PropertiesTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case ILogEntryViewModel _:
                    return LogEntryTemplate;
                case LogEntryProperty _:
                    return PropertiesTemplate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
    }
}
