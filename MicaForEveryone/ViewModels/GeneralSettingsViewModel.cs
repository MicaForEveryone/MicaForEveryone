using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Windows.UI.Xaml;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : BaseViewModel, IGeneralSettingsViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IStartupService _startupService;

        public GeneralSettingsViewModel(ISettingsService settingsService, IStartupService startupService)
        {
            _settingsService = settingsService;
            _startupService = startupService;
        }

        public bool ReloadOnChange
        {
            get => _settingsService.ConfigFile.IsFileWatcherEnabled;
            set
            {
                _settingsService.ConfigFile.IsFileWatcherEnabled = value;
                _settingsService.RaiseChanged(SettingsChangeType.ConfigFileWatcherStateChanged, null);
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

        public string ConfigFilePath
        {
            get => _settingsService.ConfigFile.FilePath;
            set
            {
                _settingsService.ConfigFile.FilePath = value;
                _settingsService.RaiseChanged(SettingsChangeType.ConfigFilePathChanged, null);
                OnPropertyChanged();
            }
        }
    }
}
