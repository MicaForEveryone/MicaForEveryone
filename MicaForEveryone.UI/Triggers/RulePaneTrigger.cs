using Windows.UI.Xaml;
using MicaForEveryone.UI.Models;
using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Core.Ui.Models;

namespace MicaForEveryone.UI.Triggers
{
    public class RulePaneTrigger : StateTriggerBase
    {
        private IPaneItem _paneItem;

        public IPaneItem PaneItem
        {
            get => _paneItem;
            set
            {
                _paneItem = value;
                SetActive(_paneItem != null &&
                    _paneItem.ItemType != PaneItemType.General);
            }
        }
    }
}
