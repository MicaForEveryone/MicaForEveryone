using System;
using System.Collections.Generic;

using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Interfaces
{
    public interface IAddClassRuleViewModel : IContentDialogViewModel
    {
        string[] Classes { get; }
        IEnumerable<string> Suggestions { get; }
        string ClassName { get; set; }

        event EventHandler Submit;
    }
}
