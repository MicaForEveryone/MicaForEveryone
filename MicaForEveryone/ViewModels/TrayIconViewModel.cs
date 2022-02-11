using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.ViewModels
{
    internal class TrayIconViewModel : BaseViewModel, ITrayIconViewModel
    {
        private readonly ISettingsService _settingsService;

        private BackdropType _backdropType;
        private TitlebarColorMode _titlebarColor;
        private bool _extendFrameIntoClientArea;
        private MainWindow _window;
        private IRule _globalRule;

        public TrayIconViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            ReloadConfigCommand = new RelyCommand(ReloadConfig);
            ChangeTitlebarColorModeCommand = new RelyCommand(ChangeTitlebarColorMode);
            ChangeBackdropTypeCommand = new RelyCommand(ChangeBackdropType);
            ExitCommand = new RelyCommand(Exit);
            EditConfigCommand = new RelyCommand(OpenConfigInEditor);
            OpenSettingsCommand = new RelyCommand(OpenSettings);

            _settingsService.Changed += SettingsService_Changed;
        }

        ~TrayIconViewModel()
        {
            _settingsService.Changed -= SettingsService_Changed;
        }

        public bool IsBackdropSupported =>
            DesktopWindowManager.IsBackdropTypeSupported;

        public bool IsMicaSupported =>
            DesktopWindowManager.IsUndocumentedMicaSupported || DesktopWindowManager.IsBackdropTypeSupported;

        public bool IsImmersiveDarkModeSupported =>
            DesktopWindowManager.IsImmersiveDarkModeSupported;

        public BackdropType BackdropType
        {
            get => _backdropType;
            set => SetProperty(ref _backdropType, value);
        }

        public TitlebarColorMode TitlebarColor
        {
            get => _titlebarColor;
            set => SetProperty(ref _titlebarColor, value);
        }

        public bool ExtendFrameIntoClientArea
        {
            get => _extendFrameIntoClientArea;
            set => SetProperty(ref _extendFrameIntoClientArea, value);
        }

        public ICommand ExitCommand { get; }

        public ICommand ReloadConfigCommand { get; }

        public ICommand ChangeTitlebarColorModeCommand { get; }

        public ICommand ChangeBackdropTypeCommand { get; }

        public ICommand EditConfigCommand { get; }

        public ICommand OpenSettingsCommand { get; }

        public async Task InitializeAsync(object sender)
        {
            _window = (MainWindow)sender;
            _window.View.ActualThemeChanged += View_ActualThemeChanged;
            _window.Destroy += Window_Destroy;

            var startupService = Program.CurrentApp.Container.GetService<IStartupService>();
            await startupService.InitializeAsync();

            _settingsService.Load();
            await _settingsService.ConfigFile.InitializeAsync();
            await _settingsService.LoadRulesAsync();

            await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var ruleService = Program.CurrentApp.Container.GetService<IRuleService>();
                var viewService = Program.CurrentApp.Container.GetService<IViewService>();

                ruleService.SystemTitlebarColorMode = viewService.SystemColorMode;
                ruleService.MatchAndApplyRuleToAllWindows();
                ruleService.StartService();
            });
        }

        public async Task ReloadConfig()
        {
            try
            {
                await _settingsService.LoadRulesAsync();
            }
            catch (ParserError error)
            {
                await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
                    dialogService.ShowErrorDialog(_window, error.Message, error.ToString(), 576, 400);
                });
            }
        }

        public void ShowContextMenu(Point offset, Rectangle notifyIconRect)
        {
            if (_window.View.ContextFlyout is MenuFlyout menu)
            {
                if (menu.IsOpen)
                {
                    menu.Hide();
                    return;
                }

                _window.SetForegroundWindow();

                _window.X = notifyIconRect.X;
                _window.Y = notifyIconRect.Y;
                _window.Width = notifyIconRect.Width;
                _window.Height = notifyIconRect.Height;
                _window.SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                menu.ShowAt(_window.View,
                    new Windows.Foundation.Point(
                        (offset.X - notifyIconRect.X) / _window.ScaleFactor,
                        (offset.Y - notifyIconRect.Y) / _window.ScaleFactor));
            }
        }

        public void ShowTooltipPopup(Rectangle notifyIconRect)
        {
            _window.X = notifyIconRect.X;
            _window.Y = notifyIconRect.Y;
            _window.Width = notifyIconRect.Width;
            _window.Height = notifyIconRect.Height;
            _window.SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            var tooltip = (ToolTip)ToolTipService.GetToolTip(_window.View);
            tooltip.IsOpen = true;
        }

        public void HideTooltipPopup()
        {
            var tooltip = (ToolTip)ToolTipService.GetToolTip(_window.View);
            tooltip.IsOpen = false;
        }

        private async void UpdateData()
        {
            _globalRule = _settingsService.Rules.First(rule => rule is GlobalRule);

            await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BackdropType = _globalRule.BackdropPreference;
                TitlebarColor = _globalRule.TitleBarColor;
                ExtendFrameIntoClientArea = _globalRule.ExtendFrameIntoClientArea;
            });
        }

        private async Task UpdateRuleAsync()
        {
            _globalRule.BackdropPreference = BackdropType;
            _globalRule.TitleBarColor = TitlebarColor;
            _globalRule.ExtendFrameIntoClientArea = ExtendFrameIntoClientArea;

            await _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, _globalRule);
        }

        private async void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            var ruleService = Program.CurrentApp.Container.GetService<IRuleService>();
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            ruleService.SystemTitlebarColorMode = viewService.SystemColorMode;
            await Task.Run(() =>
            {
                ruleService.MatchAndApplyRuleToAllWindows();
            });
        }

        private void SettingsService_Changed(object sender, SettingsChangedEventArgs args)
        {
            if ((args.Type == SettingsChangeType.RuleChanged && args.Rule is GlobalRule) 
                || args.Type == SettingsChangeType.ConfigFileReloaded)
            {
                UpdateData();
            }
        }

        private void Window_Destroy(object sender, WndProcEventArgs e)
        {
            var ruleService = Program.CurrentApp.Container.GetService<IRuleService>();
            ruleService.StopService();
        }

        // commands

        private async void ReloadConfig(object parameter)
        {
            await ReloadConfig();
        }

        private async void ChangeTitlebarColorMode(object parameter)
        {
            TitlebarColor = parameter.ToString() switch
            {
                "Default" => TitlebarColorMode.Default,
                "System" => TitlebarColorMode.System,
                "Light" => TitlebarColorMode.Light,
                "Dark" => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            await UpdateRuleAsync();
        }

        private async void ChangeBackdropType(object parameter)
        {
            BackdropType = parameter.ToString() switch
            {
                "Default" => BackdropType.Default,
                "None" => BackdropType.None,
                "Mica" => BackdropType.Mica,
                "Acrylic" => BackdropType.Acrylic,
                "Tabbed" => BackdropType.Tabbed,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            await UpdateRuleAsync();
        }

        private void Exit(object obj)
        {
            _window.Close();
        }

        private void OpenConfigInEditor(object obj)
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

        private void OpenSettings(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.ShowSettingsWindow();
        }
    }
}
