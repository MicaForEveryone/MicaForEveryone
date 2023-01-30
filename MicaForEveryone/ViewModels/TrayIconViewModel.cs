using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Core.Models;
using MicaForEveryone.Core.Ui.ViewModels;
using MicaForEveryone.Core.Ui.Views;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class TrayIconViewModel : ObservableObject, ITrayIconViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IRuleService _ruleService;
        private readonly IViewService _appLifeTimeService;

        private ITrayIconView? _view;
        private GlobalRule? _globalRule;
        private bool _trayIconVisible;

        public TrayIconViewModel(ISettingsService settingsService, IRuleService ruleService, IViewService appLifeTimeService)
        {
            _settingsService = settingsService;
            _ruleService = ruleService;
            _appLifeTimeService = appLifeTimeService;

            _trayIconVisible = _settingsService.TrayIconVisibility;
            _settingsService.TrayIconVisibilityChanged += SettingsService_TrayIconVisibilityChanged;
            _settingsService.RuleChanged += SettingsService_ConfigFileReloaded;
            _settingsService.ConfigFileReloaded += SettingsService_ConfigFileReloaded;

            ReloadConfigAsyncCommand = new AsyncRelayCommand(DoReloadConfigAsync);
            ChangeTitlebarColorModeAsyncCommand = new AsyncRelayCommand<string>(DoChangeTitlebarColorModeAsync);
            ChangeBackdropTypeAsyncCommand = new AsyncRelayCommand<string>(DoChangeBackdropTypeAsync);
            ChangeCornerPreferenceAsyncCommand = new AsyncRelayCommand<string>(DoChangeCornerPreferenceAsync);
            ExitCommand = new RelayCommand(DoExit);
            EditConfigCommand = new RelayCommand(DoOpenConfigInEditor);
            OpenSettingsCommand = new RelayCommand(DoOpenSettings);
        }

        ~TrayIconViewModel()
        {
            _settingsService.TrayIconVisibilityChanged -= SettingsService_TrayIconVisibilityChanged;
            _settingsService.RuleChanged -= SettingsService_ConfigFileReloaded;
            _settingsService.ConfigFileReloaded -= SettingsService_ConfigFileReloaded;
        }

        // properties

        public bool IsBackdropSupported => DesktopWindowManager.IsBackdropTypeSupported;
        public bool IsMicaSupported => DesktopWindowManager.IsUndocumentedMicaSupported;
        public bool IsImmersiveDarkModeSupported => DesktopWindowManager.IsImmersiveDarkModeSupported;
        public bool IsCornerPreferenceSupported => DesktopWindowManager.IsCornerPreferenceSupported;

        public BackdropType BackdropType => GlobalRule?.BackdropPreference ?? default;
        public TitlebarColorMode TitlebarColor => GlobalRule?.TitleBarColor ?? default;
        public CornerPreference CornerPreference => GlobalRule?.CornerPreference ?? default;

        private GlobalRule? GlobalRule
        {
            get => _globalRule;
            set
            {
                if (_globalRule != value)
                {
                    _globalRule = value;
                    OnPropertyChanged(nameof(BackdropType));
                    OnPropertyChanged(nameof(TitlebarColor));
                }
            }
        }

        public bool TrayIconVisible
        {
            get => _trayIconVisible;
            set => SetProperty(ref _trayIconVisible, value);
        }

        // commands

        public IAsyncRelayCommand ChangeTitlebarColorModeAsyncCommand { get; }
        public IAsyncRelayCommand ChangeBackdropTypeAsyncCommand { get; }
        public IAsyncRelayCommand ChangeCornerPreferenceAsyncCommand { get; }

        public IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        public ICommand EditConfigCommand { get; }

        public ICommand OpenSettingsCommand { get; }
        public ICommand ExitCommand { get; }

        // public methods

        public void Attach(ITrayIconView view)
        {
            // initialize view model
            _view = view;

            if (_view is not XamlWindow window) return;
            
            window.Destroy += MainWindow_Destroy;
            window.View.ActualThemeChanged += View_ActualThemeChanged;
            _ruleService.SystemTitlebarColorMode = window.View.ActualTheme switch
            {
                ElementTheme.Light => TitlebarColorMode.Light,
                ElementTheme.Dark => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        // event handlers
        
        private void SettingsService_TrayIconVisibilityChanged(object? sender, EventArgs e)
        {
            TrayIconVisible = _settingsService.TrayIconVisibility;
        }

        private void SettingsService_ConfigFileReloaded(object? sender, EventArgs args)
        {
            _appLifeTimeService.DispatcherEnqueue(() =>
            {
                if (args is RulesChangeEventArgs ruleChangeArgs)
                {
                    if (GlobalRule == ruleChangeArgs.Rule)
                    {
                        OnPropertyChanged(nameof(BackdropType));
                        OnPropertyChanged(nameof(TitlebarColor));
                    }
                    else if (ruleChangeArgs.Rule is GlobalRule globalRule)
                    {
                        GlobalRule = globalRule;
                    }
                }
                else
                {
                    GlobalRule = (GlobalRule)_settingsService.Rules.First(rule => rule is GlobalRule);
                }
            });
        }

        private void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            _ruleService.SystemTitlebarColorMode = sender.ActualTheme switch
            {
                ElementTheme.Light => TitlebarColorMode.Light,
                ElementTheme.Dark => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(),
            };

            _ = _ruleService.MatchAndApplyRuleToAllWindowsAsync();
        }

        private void MainWindow_Destroy(object? sender, WndProcEventArgs args)
        {
            _ruleService.StopService();
        }

        // commands

        private async Task DoReloadConfigAsync()
        {
            await _settingsService.LoadRulesAsync();
        }

        private async Task DoChangeTitlebarColorModeAsync(string? parameter)
        {
            var value = parameter switch
            {
                "Default" => TitlebarColorMode.Default,
                "System" => TitlebarColorMode.System,
                "Light" => TitlebarColorMode.Light,
                "Dark" => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };

            if (GlobalRule == null) return;
            GlobalRule.TitleBarColor = value;
            await _settingsService.UpdateRuleAsync(GlobalRule);
        }

        private async Task DoChangeBackdropTypeAsync(string? parameter)
        {
            var value = parameter switch
            {
                "Default" => BackdropType.Default,
                "None" => BackdropType.None,
                "Mica" => BackdropType.Mica,
                "Acrylic" => BackdropType.Acrylic,
                "Tabbed" => BackdropType.Tabbed,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            if (GlobalRule == null) return;
            GlobalRule.BackdropPreference = value;
            await _settingsService.UpdateRuleAsync(GlobalRule);
        }

        private async Task DoChangeCornerPreferenceAsync(string? parameter)
        {
            var value = parameter switch
            {
                "Default" => CornerPreference.Default,
                "Square" => CornerPreference.Square,
                "Rounded" => CornerPreference.Rounded,
                "RoundedSmall" => CornerPreference.RoundedSmall,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            if (GlobalRule == null) return;
            GlobalRule.CornerPreference = value;
            await _settingsService.UpdateRuleAsync(GlobalRule);
        }

        private void DoExit()
        {
            _view?.Close();
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

        private void DoOpenSettings()
        {
            var viewService = Program.Container.GetService<IViewService>();
            viewService?.ShowSettingsWindow();
        }
    }
}
