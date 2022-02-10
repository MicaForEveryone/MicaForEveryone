using System.ComponentModel;

namespace MicaForEveryone.UI.ViewModels
{
    public interface IGeneralSettingsViewModel : INotifyPropertyChanged
    {
        bool ReloadOnChange { get; set; }
        bool RunOnStartup { get; set; }
        bool RunOnStartupAvailable { get; }
        string ConfigFilePath { get; set; }
    }
}
