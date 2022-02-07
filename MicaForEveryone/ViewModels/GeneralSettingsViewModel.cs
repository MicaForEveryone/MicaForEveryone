using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : BaseViewModel, IGeneralSettingsViewModel
    {
        private readonly IConfigService _configService;
        private readonly IStartupService _startupService;

        private bool _runOnStartup;

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
            get => _runOnStartup;
            set => SetRunOnStartup(value);
        }

        public async void Initialize()
        {
            var state = await _startupService.GetEnabledAsync();
            SetProperty(ref _runOnStartup, state, nameof(RunOnStartup));
        }

        public async void SetRunOnStartup(bool value)
        {
            var state = await _startupService.SetEnabledAsync(value);
            SetProperty(ref _runOnStartup, state, nameof(RunOnStartup));
        }
    }
}
