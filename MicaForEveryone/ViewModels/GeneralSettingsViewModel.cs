using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Xaml;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : ObservableObject, IGeneralSettingsViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IStartupService _startupService;

        private XamlWindow? _window;

        public GeneralSettingsViewModel(ISettingsService settingsService, IStartupService startupService)
        {
            _settingsService = settingsService;
            _startupService = startupService;
            BrowseAsyncCommand = new AsyncRelayCommand(DoBrowseAsync);

            _settingsService.Changed += SettingsService_Changed;
        }

        ~GeneralSettingsViewModel()
        {
            _settingsService.Changed -= SettingsService_Changed;
        }

        public void Initialize(XamlWindow sender)
        {
            _window = sender;
        }

        public bool ReloadOnChange
        {
            get => _settingsService.ConfigFile.IsFileWatcherEnabled;
            set
            {
                if (_settingsService.ConfigFile.IsFileWatcherEnabled != value)
                {
                    _settingsService.ConfigFile.IsFileWatcherEnabled = value;
                    _settingsService.CommitChangesAsync(SettingsChangeType.ConfigFileWatcherStateChanged, null);
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
                    _startupService.SetStateAsync(value);
                    // FIXME: OnPropertyChanged
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
                    _settingsService.CommitChangesAsync(SettingsChangeType.ConfigFilePathChanged, null);
                }
            }
        }

        public IAsyncRelayCommand BrowseAsyncCommand { get; }

        // event handlers

        private async void SettingsService_Changed(object? sender, SettingsChangedEventArgs args)
        {
            await _window?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Type)
                {
                    case SettingsChangeType.ConfigFilePathChanged:
                        OnPropertyChanged(nameof(ConfigFilePath));
                        break;

                    case SettingsChangeType.ConfigFileWatcherStateChanged:
                        OnPropertyChanged(nameof(ReloadOnChange));
                        break;

                    case SettingsChangeType.RuleAdded:
                    case SettingsChangeType.RuleRemoved:
                    case SettingsChangeType.RuleChanged:
                    case SettingsChangeType.ConfigFileReloaded:
                        break;
                }
            });
        }

        // commands

        private async Task DoBrowseAsync()
        {
            // create picker
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

            // initialize picker with parent window
            ((IInitializeWithWindow)(object)picker).Initialize(_window!.Interop.WindowHandle);

            // ask user to pick a file
            var file = await picker.PickSingleFileAsync();

            // change config file path if user picked a file
            if (file != null)
            {
                _settingsService.ConfigFile.FilePath = file.Path;
                await _settingsService.CommitChangesAsync(SettingsChangeType.ConfigFilePathChanged, null);
            }
        }
    }
}
