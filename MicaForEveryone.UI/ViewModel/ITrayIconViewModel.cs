using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.UI.ViewModels
{
    public interface ITrayIconViewModel : INotifyPropertyChanged
    {
        bool IsBackdropSupported { get; }
        bool IsMicaSupported { get; }
        bool IsImmersiveDarkModeSupported { get; }

        BackdropType BackdropType { get; }
        TitlebarColorMode TitlebarColor { get; }

        IAsyncRelayCommand ChangeTitlebarColorModeAsyncCommand { get; }
        IAsyncRelayCommand ChangeBackdropTypeAsyncCommand { get; }
        
        IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        ICommand EditConfigCommand { get; }

        ICommand OpenSettingsCommand { get; }
        ICommand ExitCommand { get; }
    }
}
