using System;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Models;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : BaseViewModel, IGeneralSettingsViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IStartupService _startupService;

        private XamlWindow _window;

        public GeneralSettingsViewModel(ISettingsService settingsService, IStartupService startupService)
        {
            _settingsService = settingsService;
            _startupService = startupService;
            BrowseCommand = new RelyCommand(Browse);
        }

        public void Initialize(object sender)
        {
            _window = sender as XamlWindow;
        }

        public bool ReloadOnChange
        {
            get => _settingsService.ConfigFile.IsFileWatcherEnabled;
            set
            {
                if (_settingsService.ConfigFile.IsFileWatcherEnabled != value)
                {
                    _settingsService.ConfigFile.IsFileWatcherEnabled = value;
                    _settingsService.RaiseChanged(SettingsChangeType.ConfigFileWatcherStateChanged, null);
                    OnPropertyChanged();
                }
            }
        }

        public bool RunOnStartup
        {
            get => _startupService.IsEnabled;
            set
            {
                if (_startupService.IsEnabled != value)
                {
                    _startupService.SetStateAsync(value).Wait();
                    OnPropertyChanged();
                }
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
                if (_settingsService.ConfigFile.FilePath != value)
                {
                    _settingsService.ConfigFile.FilePath = value;
                    _settingsService.RaiseChanged(SettingsChangeType.ConfigFilePathChanged, null);
                    OnPropertyChanged();
                }
            }
        }

        public ICommand BrowseCommand { get; }

        public async void Browse(object parameter)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.ComputerFolder,
                FileTypeFilter =
                {
                    ".conf",
                    ".xcl",
                },
            };
            ((IInitializeWithWindow)(object)picker).Initialize(_window.Interop.WindowHandle);
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _settingsService.ConfigFile.FilePath = file.Path;
                _settingsService.RaiseChanged(SettingsChangeType.ConfigFilePathChanged, null);
            }
        }
    }
}
