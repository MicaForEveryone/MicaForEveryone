using MicaForEveryone.Core.Ui.Models;

namespace MicaForEveryone.Core.Ui.Interfaces;

public interface IPaneItem
{
    PaneItemType ItemType { get; }
}