using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Core.Ui.ViewModels;

namespace MicaForEveryone.Core.Ui.Models;

public class GeneralPaneItem : IPaneItem
{
    public GeneralPaneItem(IGeneralSettingsViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    public PaneItemType ItemType => PaneItemType.General;
    public IGeneralSettingsViewModel ViewModel { get; }
}