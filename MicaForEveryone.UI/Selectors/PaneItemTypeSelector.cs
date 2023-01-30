using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MicaForEveryone.UI.Models;
using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Core.Ui.Models;

namespace MicaForEveryone.UI.Selectors
{
    internal class PaneItemTypeSelector : DataTemplateSelector
    {
        public DataTemplate General { get; set; }
        public DataTemplate Global { get; set; }
        public DataTemplate Process { get; set; }
        public DataTemplate Class { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var paneItem = (IPaneItem)item;

            switch (paneItem?.ItemType)
            {
                case PaneItemType.General:
                    return General;
                case PaneItemType.Global:
                    return Global;
                case PaneItemType.Process:
                    return Process;
                case PaneItemType.Class:
                    return Class;
                default:
                    return null;
            }
        }
    }
}
