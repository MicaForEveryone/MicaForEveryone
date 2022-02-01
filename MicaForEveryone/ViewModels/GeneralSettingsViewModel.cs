using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : BaseViewModel, IGeneralSettingsViewModel
    {
        private readonly IConfigService _configService;

        public GeneralSettingsViewModel(IConfigService configService)
        {
            _configService = configService;
        }

        public bool ReloadOnChange
        {
            get
            {
                return _configService.ConfigSource.GetWatchState();
            }
            set
            {
                _configService.ConfigSource.SetWatchState(value);
                OnPropertyChanged();
            }
        }

        public bool RunOnStartup
        {
            get => false;
            set { }
        }
    }
}
