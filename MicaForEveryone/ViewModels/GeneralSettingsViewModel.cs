using System;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : BaseViewModel, IGeneralSettingsViewModel
    {
        private readonly IConfigService _configService;
        private readonly IStartupService _startupService;

        public GeneralSettingsViewModel(IConfigService configService, IStartupService startupService)
        {
            _configService = configService;
            _startupService = startupService;
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
            get => _startupService.IsEnabled;
            set
            {
                _startupService.SetStateAsync(value).ContinueWith(async result =>
                {
                    var viewService = Program.CurrentApp.Container.GetService<IViewService>();
                    await viewService.SettingsWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        OnPropertyChanged(nameof(RunOnStartup));
                    });
                });
            }
        }

        public bool RunOnStartupAvailable
        {
            get => _startupService.IsAvailable;
        }
    }
}
