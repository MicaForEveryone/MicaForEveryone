using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MicaForEveryone.ViewModels
{
    public interface IGeneralSettingsViewModel : INotifyPropertyChanged
    {
        bool ReloadOnChange { get; set; }
        bool RunOnStartup { get; set; }
    }
}
