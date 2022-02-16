using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.UI.Models
{
    public class GeneralPaneItem : IPaneItem
    {
        public GeneralPaneItem(IGeneralSettingsViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public PaneItemType ItemType => PaneItemType.General;
        public IGeneralSettingsViewModel ViewModel { get; }
    }
}
