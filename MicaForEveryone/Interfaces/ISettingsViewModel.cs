using System.Threading.Tasks;

using MicaForEveryone.Views;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    internal interface ISettingsViewModel : UI.ViewModels.ISettingsViewModel
    {
        void Initialize(SettingsWindow sender);
    }
}
