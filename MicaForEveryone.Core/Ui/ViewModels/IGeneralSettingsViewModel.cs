using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Core.Ui.Views;

namespace MicaForEveryone.Core.Ui.ViewModels
{
    public interface IGeneralSettingsViewModel : INotifyPropertyChanged
    {
        IReadOnlyList<object>? Languages { get; }
        object SelectedLanguage { get; set; }

        bool ReloadOnChange { get; set; }
        bool RunOnStartup { get; set; }
        bool RunOnStartupAvailable { get; }
        bool RunOnStartupAsAdmin { get; set; }
        bool RunOnStartupAsAdminAvailable { get; }
        string? ConfigFilePath { get; set; }
        bool TrayIconVisibility { get; set; }

        IAsyncRelayCommand BrowseAsyncCommand { get; }

        ICommand EditConfigCommand { get; }
        IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        IAsyncRelayCommand ResetConfigAsyncCommand { get; }
        ICommand ExitCommand { get; }
        
        void Attach(ISettingsView view);
    }
}
