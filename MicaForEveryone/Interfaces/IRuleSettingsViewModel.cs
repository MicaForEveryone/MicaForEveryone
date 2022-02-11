#nullable enable

namespace MicaForEveryone.Interfaces
{
    internal interface IRuleSettingsViewModel : UI.ViewModels.IRuleSettingsViewModel
    {
        IRule? Rule { get; set; }
    }
}
