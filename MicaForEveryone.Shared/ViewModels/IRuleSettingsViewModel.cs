using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
    public interface IRuleSettingsViewModel : INotifyPropertyChanged
    {
        BackdropType BackdropType { get; set; }
        TitlebarColorMode TitlebarColor { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }

        ISettingsViewModel ParentViewModel { get; set; }

        ICommand SaveCommand { get; }

        void InitializeData(object data);
    }
}
