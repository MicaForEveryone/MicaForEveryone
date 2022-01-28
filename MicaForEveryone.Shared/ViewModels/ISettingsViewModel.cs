using System.ComponentModel;
using System.Windows.Input;

namespace MicaForEveryone.ViewModels
{
    public interface ISettingsViewModel : INotifyPropertyChanged
    {
        bool ReloadOnChange { get; set; }

        ICommand CloseCommand { get; }
    }
}
