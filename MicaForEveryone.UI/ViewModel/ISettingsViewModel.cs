using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using MicaForEveryone.Models;
using MicaForEveryone.UI.Models;

namespace MicaForEveryone.UI.ViewModels
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        bool IsBackdropSupported { get; }
        bool IsMicaSupported { get; }
        bool IsImmersiveDarkModeSupported { get; }

        Version Version { get; }

        ObservableCollection<BackdropType> BackdropTypes { get; }
        ObservableCollection<TitlebarColorMode> TitlebarColorModes { get; }

        ObservableCollection<IPaneItem> PaneItems { get; set; }
        IPaneItem SelectedPane { get; set; }

        ICommand CloseCommand { get; }
        ICommand AddProcessRuleCommand { get; }
        ICommand AddClassRuleCommand { get; }
        ICommand RemoveRuleCommand { get; }
        ICommand EditConfigCommand { get; }
        ICommand ReloadConfigCommand { get; }
        ICommand ResetConfigCommand { get; }

        void Initialize(object sender);
    }
}
