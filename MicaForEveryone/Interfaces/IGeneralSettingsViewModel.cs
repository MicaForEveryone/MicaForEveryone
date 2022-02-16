using MicaForEveryone.Xaml;

namespace MicaForEveryone.Interfaces
{
    internal interface IGeneralSettingsViewModel : UI.ViewModels.IGeneralSettingsViewModel
    {
        void Initialize(XamlWindow sender);
    }
}
