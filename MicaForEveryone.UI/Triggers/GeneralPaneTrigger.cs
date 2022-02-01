using Windows.UI.Xaml;

using MicaForEveryone.Models;

namespace MicaForEveryone.UI.Triggers
{
    public class GeneralPaneTrigger : StateTriggerBase
    {
        private IPaneItem _paneItem;

        public IPaneItem PaneItem
        {
            get => _paneItem;
            set
            {
                _paneItem = value;
                SetActive(_paneItem?.ItemType == PaneItemType.General);
            }
        }
    }
}
