using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Storage.Pickers;
using Windows.UI.Core;
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
        private readonly ILanguageService _languageService;

        private XamlWindow? _window;
        private Language _currentLanguage;

        public GeneralSettingsViewModel(ISettingsService settingsService, IStartupService startupService, ILanguageService languageService)
        {
            _settingsService = settingsService;
            _startupService = startupService;
            _languageService = languageService;

            _currentLanguage = _languageService.CurrentLanguage;
            Languages = _languageService.SupportedLanguages;
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

        public IList<object> Languages { get; }

        public object SelectedLanguage
        {
            get => _currentLanguage;
            set
            {
                var language = value as Language;
                if (language != null && _currentLanguage.LanguageTag != language.LanguageTag)
                {
                    _languageService.SetLanguage(language);
                    _settingsService.Save();
                    SetProperty(ref _currentLanguage, language);
                }
            }
        }

        public IAsyncRelayCommand BrowseAsyncCommand { get; }

        // event handlers

        private void SettingsService_Changed(object? sender, SettingsChangedEventArgs args)
        {
            Program.CurrentApp.Dispatcher.Enqueue(() =>
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
