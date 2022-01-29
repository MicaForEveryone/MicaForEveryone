using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        bool ReloadOnChange { get; set; }
        bool SystemBackdropIsSupported { get; }
        BackdropType BackdropType { get; set; }
        TitlebarColorMode TitlebarColor { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }

        ObservableCollection<BackdropType> BackdropTypesSource { get; }
        ObservableCollection<TitlebarColorMode> TitlebarColorModesSource { get; }

        ICommand CloseCommand { get; }
    }
}
