using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

            // initialize other services
            var ctx = Program.CurrentApp.Container;

            var svcStartup = ctx.GetService<IStartupService>()!;
            var initStartup = svcStartup.InitializeAsync();

            _settingsService.Load();
            await _settingsService.ConfigFile.InitializeAsync();
            var initRules = _settingsService.LoadRulesAsync();

            // start rule service when everything is ready
            await Task.WhenAll(initStartup, initRules);
            _ruleService.MatchAndApplyRuleToAllWindows();
            _ruleService.StartService();
        }

        // event handlers

        private async void Settings_Changed(object? sender, SettingsChangedEventArgs args)
        {
            if ((args.Type == SettingsChangeType.RuleChanged && args.Rule is GlobalRule)
                || args.Type == SettingsChangeType.ConfigFileReloaded)
            {
                await _mainWindow?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (GlobalRule == args.Rule)
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

        private async void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            _ruleService.SystemTitlebarColorMode = sender.ActualTheme switch
            {
                ElementTheme.Light => TitlebarColorMode.Light,
                ElementTheme.Dark => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(),
            };
            await Task.Run(() => _ruleService.MatchAndApplyRuleToAllWindows());
        }

        private void MainWindow_Destroy(object? sender, WndProcEventArgs args)
        {
            _ruleService.StopService();
        }

        // commands

        private async Task DoReloadConfigAsync()
        {
            try
            {
                await _settingsService.LoadRulesAsync();
            }
            catch (ParserError error)
            {
                await _mainWindow?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
                    dialogService?.ShowErrorDialog(_mainWindow, error.Message, error.ToString(), 576, 400);
                });
            }
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
