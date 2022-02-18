using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace MicaForEveryone.UI.ViewModels
{
    public interface IGeneralSettingsViewModel : INotifyPropertyChanged
    {
        IList<object> Languages { get; }
        object SelectedLanguage { get; set; }

        bool ReloadOnChange { get; set; }
        bool RunOnStartup { get; set; }
        bool RunOnStartupAvailable { get; }
        string ConfigFilePath { get; set; }

        IAsyncRelayCommand BrowseAsyncCommand { get; }

        ICommand EditConfigCommand { get; }
        IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        IAsyncRelayCommand ResetConfigAsyncCommand { get; }
    }
}
