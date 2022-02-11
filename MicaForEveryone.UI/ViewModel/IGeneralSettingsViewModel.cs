using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace MicaForEveryone.UI.ViewModels
{
    public interface IGeneralSettingsViewModel : INotifyPropertyChanged
    {
        bool ReloadOnChange { get; set; }
        bool RunOnStartup { get; set; }
        bool RunOnStartupAvailable { get; }
        string ConfigFilePath { get; set; }

        IAsyncRelayCommand BrowseAsyncCommand { get; }
    }
}
