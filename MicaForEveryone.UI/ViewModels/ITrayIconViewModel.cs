using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.UI.ViewModels
{
    public interface ITrayIconViewModel : INotifyPropertyChanged
    {
        bool IsBackdropSupported { get; }
        bool IsMicaSupported { get; }
        bool IsImmersiveDarkModeSupported { get; }
        bool IsCornerPreferenceSupported { get; }

        BackdropType BackdropType { get; }
        TitlebarColorMode TitlebarColor { get; }
        CornerPreference CornerPreference { get; }

        IAsyncRelayCommand ChangeTitlebarColorModeAsyncCommand { get; }
        IAsyncRelayCommand ChangeBackdropTypeAsyncCommand { get; }
        IAsyncRelayCommand ChangeCornerPreferenceAsyncCommand { get; }
        
        IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        ICommand EditConfigCommand { get; }

        ICommand OpenLogsCommand { get; }
        ICommand OpenSettingsCommand { get; }
        ICommand ExitCommand { get; }
    }
}
