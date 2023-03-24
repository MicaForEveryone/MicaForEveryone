using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Core.Ui.ViewModels;

namespace MicaForEveryone.Core.Ui.Models;

public class RulePaneItem : IPaneItem
{
    public RulePaneItem(string title, PaneItemType type, IRuleSettingsViewModel viewModel)
    {
        Title = title;
        ItemType = type;
        ViewModel = viewModel;
    }

    public string Title { get; }
    public PaneItemType ItemType { get; }
    public IRuleSettingsViewModel ViewModel { get; }

    public override bool Equals(object obj)
    {
        if (obj is RulePaneItem rulePaneItem)
        {
            return rulePaneItem.ItemType == ItemType &&
                   rulePaneItem.Title == Title;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}