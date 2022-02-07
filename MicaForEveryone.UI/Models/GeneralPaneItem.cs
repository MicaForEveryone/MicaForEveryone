using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.UI.Models
{
    public class GeneralPaneItem : IPaneItem
    {
        public GeneralPaneItem(IGeneralSettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewModel.Initialize();
        }

        public PaneItemType ItemType { get; } = PaneItemType.General;
        public IGeneralSettingsViewModel ViewModel { get; }
    }
}
