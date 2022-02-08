using System;
using System.Collections.Generic;

using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Interfaces
{
    public interface IAddProcessRuleViewModel : IContentDialogViewModel
    {
        string[] Processes { get; }
        IEnumerable<string> Suggestions { get; }
        string ProcessName { get; set; }

        event EventHandler Submit;
    }
}
