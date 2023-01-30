using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Core.Models;
using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Core.Ui.Views;

namespace MicaForEveryone.Core.Ui.ViewModels
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

        void Attach(ISettingsView view);
    }
}
