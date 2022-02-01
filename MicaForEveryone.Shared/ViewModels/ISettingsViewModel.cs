using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        bool SystemBackdropIsSupported { get; }

        Version Version { get; }

        ObservableCollection<BackdropType> BackdropTypes { get; }
        ObservableCollection<TitlebarColorMode> TitlebarColorModes { get; }

        ObservableCollection<IPaneItem> PaneItems { get; set; }
        IPaneItem SelectedPane { get; set; }

        ICommand CloseCommand { get; }
        ICommand AddProcessRuleCommand { get; }
        ICommand AddClassRuleCommand { get; }
        ICommand RemoveRuleCommand { get; }

        void Initialize(object sender);
    }
}
