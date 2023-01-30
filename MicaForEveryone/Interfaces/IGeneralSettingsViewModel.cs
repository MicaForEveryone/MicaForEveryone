using MicaForEveryone.Xaml;

namespace MicaForEveryone.Interfaces
{
    internal interface IGeneralSettingsViewModel : Core.Ui.ViewModels.IGeneralSettingsViewModel
    {
        void Initialize(XamlWindow sender);
    }
}
