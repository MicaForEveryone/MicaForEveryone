using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Globalization;
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
        private readonly ILanguageService _languageService;
        private readonly ITaskSchedulerService _taskSchedulerService;

        private XamlWindow? _window;
        private Win32.Window? _mainWindow;
        private Language _currentLanguage;

        public GeneralSettingsViewModel(ISettingsService settingsService, IStartupService startupService, ILanguageService languageService, ITaskSchedulerService taskSchedulerService, IViewService viewService)
        {
            _settingsService = settingsService;
            _startupService = startupService;
            _languageService = languageService;
            _taskSchedulerService = taskSchedulerService;

            _mainWindow = viewService.MainWindow;

            _currentLanguage = _languageService.CurrentLanguage;
            Languages = _languageService.SupportedLanguages;
            BrowseAsyncCommand = new AsyncRelayCommand(DoBrowseAsync);

            ReloadConfigAsyncCommand = new AsyncRelayCommand(DoReloadConfigAsync);
            EditConfigCommand = new RelayCommand(DoOpenConfigInEditor);
            ResetConfigAsyncCommand = new AsyncRelayCommand(DoResetConfigAsync);
            ExitCommand = new RelayCommand(DoExit);

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

        public bool RunOnStartupAsAdmin
        {
            get => _taskSchedulerService.IsRunAsAdminTaskEnabled();
            set
            {
                if (_taskSchedulerService.IsRunAsAdminTaskEnabled() != value)
                {
                    if (value)
                    {
                        _taskSchedulerService.CreateRunAsAdminTask();
                    }
                    else
                    {
                        _taskSchedulerService.RemoveRunAsAdminTask();
                    }
                }
            }
        }

        public bool RunOnStartupAsAdminAvailable
        {
            get => _taskSchedulerService.IsAvailable();
        }

        public bool TrayIconVisibility
        {
            get => _settingsService.TrayIconVisibility;
            set
            {
                if (_settingsService.TrayIconVisibility != value)
                {
                    _settingsService.TrayIconVisibility = value;
                }
            }
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

        public ICommand EditConfigCommand { get; }
        public IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        public IAsyncRelayCommand ResetConfigAsyncCommand { get; }
        public ICommand ExitCommand { get; }

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

        private async Task DoReloadConfigAsync()
        {
            await _settingsService.LoadRulesAsync();
        }

        private void DoOpenConfigInEditor()
        {
            var startInfo = new ProcessStartInfo(_settingsService.ConfigFile.FilePath)
            {
                UseShellExecute = true
            };
            if (startInfo.Verbs.Contains("edit"))
            {
                startInfo.Verb = "edit";
            }
            Process.Start(startInfo);
        }

        private async Task DoResetConfigAsync()
        {
            await _settingsService.ConfigFile.ResetAsync();
            await _settingsService.LoadRulesAsync();
        }

        private void DoExit()
        {
            _window?.Close();
            _mainWindow?.Close();
        }
    }
}
