using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class TrayIconViewModel : ObservableObject, ITrayIconViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IRuleService _ruleService;

        private MainWindow? _mainWindow;
        private GlobalRule? _globalRule;

        public TrayIconViewModel(ISettingsService settingsService, IRuleService ruleService)
        {
            _settingsService = settingsService;
            _ruleService = ruleService;

            _settingsService.Changed += Settings_Changed;

            ReloadConfigAsyncCommand = new AsyncRelayCommand(DoReloadConfigAsync);
            ChangeTitlebarColorModeAsyncCommand = new AsyncRelayCommand<string>(DoChangeTitlebarColorModeAsync);
            ChangeBackdropTypeAsyncCommand = new AsyncRelayCommand<string>(DoChangeBackdropTypeAsync);
            ExitCommand = new RelayCommand(DoExit);
            EditConfigCommand = new RelayCommand(DoOpenConfigInEditor);
            OpenSettingsCommand = new RelayCommand(DoOpenSettings);
        }

        ~TrayIconViewModel()
        {
            _settingsService.Changed -= Settings_Changed;
        }

        // properties

        public bool IsBackdropSupported => DesktopWindowManager.IsBackdropTypeSupported;
        public bool IsMicaSupported => DesktopWindowManager.IsUndocumentedMicaSupported;
        public bool IsImmersiveDarkModeSupported => DesktopWindowManager.IsImmersiveDarkModeSupported;

        public BackdropType BackdropType => GlobalRule?.BackdropPreference ?? default;
        public TitlebarColorMode TitlebarColor => GlobalRule?.TitleBarColor ?? default;

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

        // commands

        public IAsyncRelayCommand ChangeTitlebarColorModeAsyncCommand { get; }
        public IAsyncRelayCommand ChangeBackdropTypeAsyncCommand { get; }

        public IAsyncRelayCommand ReloadConfigAsyncCommand { get; }
        public ICommand EditConfigCommand { get; }

        public ICommand OpenSettingsCommand { get; }
        public ICommand ExitCommand { get; }

        // public methods

        public async Task InitializeAsync(MainWindow sender)
        {
            // initialize view model
            _mainWindow = sender;
            _mainWindow.Destroy += MainWindow_Destroy;

            // initialize rule service
            _mainWindow.View.ActualThemeChanged += View_ActualThemeChanged;
            _ruleService.SystemTitlebarColorMode = _mainWindow.View.ActualTheme switch
            {
                ElementTheme.Light => TitlebarColorMode.Light,
                ElementTheme.Dark => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(),
            };

            // initialize and load config file
            await _settingsService.ConfigFile.InitializeAsync();
            await _settingsService.LoadRulesAsync();

            // initialize startup service
            var startupService = Program.CurrentApp.Container.GetRequiredService<IStartupService>();
            _ = startupService.InitializeAsync();

            // start rule service
            await _ruleService.MatchAndApplyRuleToAllWindowsAsync();

            // need to be started on UI thread
            Program.CurrentApp.Dispatcher.Enqueue(() =>
            {
                _ruleService.StartService();
            });

            // post a message to window to invoke dispatcher
            sender.PostMessage(Win32.PInvoke.WindowMessage.WM_NULL);
        }

        // event handlers

        private void Settings_Changed(object? sender, SettingsChangedEventArgs args)
        {
            if ((args.Type == SettingsChangeType.RuleChanged && args.Rule is GlobalRule)
                || args.Type == SettingsChangeType.ConfigFileReloaded)
            {
                Program.CurrentApp.Dispatcher.Enqueue(() =>
                {
                    if (args.Type == SettingsChangeType.ConfigFileReloaded)
                    {
                        GlobalRule = _settingsService.ConfigFile.Parser.Rules.First(
                            rule => rule is GlobalRule) as GlobalRule;
                    }
                    else if (GlobalRule == args.Rule)
                    {
                        OnPropertyChanged(nameof(BackdropType));
                        OnPropertyChanged(nameof(TitlebarColor));
                    }
                    else
                    {
                        GlobalRule = args.Rule as GlobalRule;
                    }
                });
            }
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
            await _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, GlobalRule);
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
            await _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, GlobalRule);
        }

        private void DoExit()
        {
            _mainWindow?.Close();
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

        private void DoOpenSettings()
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService?.ShowSettingsWindow();
        }
    }
}
