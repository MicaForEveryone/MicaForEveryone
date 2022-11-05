using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Models;
using MicaForEveryone.UI.Models;

namespace MicaForEveryone.UI.ViewModels
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        bool IsBackdropSupported { get; }
        bool IsMicaSupported { get; }
        bool IsImmersiveDarkModeSupported { get; }

        string Version { get; }

        IList<BackdropType> BackdropTypes { get; }
        IList<TitlebarColorMode> TitlebarColorModes { get; }
        IList<CornerPreference> CornerPreferences { get; }

        IList<IPaneItem> PaneItems { get; set; }
        IPaneItem SelectedPane { get; set; }

        ICommand CloseCommand { get; }

        ICommand AddProcessRuleCommand { get; }
        ICommand AddClassRuleCommand { get; }
        IAsyncRelayCommand RemoveRuleAsyncCommand { get; }
    }
}
