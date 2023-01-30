using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Core.Models;
using MicaForEveryone.Core.Ui.Views;

namespace MicaForEveryone.Core.Ui.ViewModels
{
    public interface ITrayIconViewModel : INotifyPropertyChanged
    {
        bool IsBackdropSupported { get; }
        bool IsMicaSupported { get; }
        bool IsImmersiveDarkModeSupported { get; }
        bool IsCornerPreferenceSupported { get; }
        bool TrayIconVisible { get; set; }

        BackdropType BackdropType { get; }
        TitlebarColorMode TitlebarColor { get; }
        CornerPreference CornerPreference { get; }

        IAsyncRelayCommand ChangeTitlebarColorModeAsyncCommand { get; }
        IAsyncRelayCommand ChangeBackdropTypeAsyncCommand { get; }
        IAsyncRelayCommand ChangeCornerPreferenceAsyncCommand { get; }

        IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        ICommand EditConfigCommand { get; }

        ICommand OpenSettingsCommand { get; }
        ICommand ExitCommand { get; }

        void Attach(ITrayIconView view);
    }
}
