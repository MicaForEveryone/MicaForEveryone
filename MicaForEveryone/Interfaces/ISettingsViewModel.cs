using System.Threading.Tasks;

using MicaForEveryone.Views;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    internal interface ISettingsViewModel : Core.Ui.ViewModels.ISettingsViewModel
    {
        void Initialize(SettingsWindow sender);
    }
}
