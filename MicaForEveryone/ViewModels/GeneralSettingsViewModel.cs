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

using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Xaml;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class GeneralSettingsViewModel : ObservableObject, IGeneralSettingsViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IUiSettingsService _uiSettingsService;
        private readonly IStartupService _startupService;
        private readonly ITaskSchedulerService _taskSchedulerService;

        private XamlWindow? _window;
        private readonly Win32.Window? _mainWindow;

        public GeneralSettingsViewModel(ISettingsService settingsService, IUiSettingsService uiSettingsService, IStartupService startupService, ILanguageService languageService, ITaskSchedulerService taskSchedulerService, IViewService viewService)
        {
            _settingsService = settingsService;
            _uiSettingsService = uiSettingsService;
            _startupService = startupService;
            _taskSchedulerService = taskSchedulerService;

            _mainWindow = viewService.MainWindow;
            
            Languages = languageService.SupportedLanguages;
            BrowseAsyncCommand = new AsyncRelayCommand(DoBrowseAsync);

            ReloadConfigAsyncCommand = new AsyncRelayCommand(DoReloadConfigAsync);
            EditConfigCommand = new RelayCommand(DoOpenConfigInEditor);
            ResetConfigAsyncCommand = new AsyncRelayCommand(DoResetConfigAsync);
            ExitCommand = new RelayCommand(DoExit);

            _settingsService.ConfigFilePathChanged += SettingsService_ConfigFilePathChanged; ;
            _settingsService.ConfigFileWatcherStateChanged += SettingsService_ConfigFileWatcherStateChanged;
        }

        ~GeneralSettingsViewModel()
        {
            _settingsService.ConfigFilePathChanged -= SettingsService_ConfigFilePathChanged; ;
            _settingsService.ConfigFileWatcherStateChanged -= SettingsService_ConfigFileWatcherStateChanged;
        }

        public void Initialize(XamlWindow sender)
        {
            _window = sender;
        }

        public bool ReloadOnChange
        {
            get => _settingsService.IsFileWatcherEnabled;
            set => _settingsService.IsFileWatcherEnabled = value;
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

        public bool RunOnStartupAsAdminAvailable => _taskSchedulerService.IsAvailable();

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

        public string? ConfigFilePath
        {
            get => _settingsService.ConfigFilePath;
            set => _settingsService.ConfigFilePath = value;
        }

        public IList<object> Languages { get; }

        public object SelectedLanguage
        {
            get => _uiSettingsService.Language;
            set
            {
                if (value is Language language)
                {
                    _uiSettingsService.Language = language;
                }
            }
        }

        public IAsyncRelayCommand BrowseAsyncCommand { get; }

        public ICommand EditConfigCommand { get; }
        public IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        public IAsyncRelayCommand ResetConfigAsyncCommand { get; }
        public ICommand ExitCommand { get; }

        // event handlers

        private void SettingsService_ConfigFilePathChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ConfigFilePath));
        }

        private void SettingsService_ConfigFileWatcherStateChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ReloadOnChange));
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
                ConfigFilePath = file.Path;
            }
        }

        private async Task DoReloadConfigAsync()
        {
            await _settingsService.LoadRulesAsync();
        }

        private void DoOpenConfigInEditor()
        {
            var startInfo = new ProcessStartInfo(_settingsService.ConfigFilePath)
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
            await _settingsService.ResetRulesAsync();
        }

        private void DoExit()
        {
            _window?.Close();
            _mainWindow?.Close();
        }
    }
}
