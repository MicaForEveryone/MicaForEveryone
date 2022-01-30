using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Views;
using MicaForEveryone.Config;
using MicaForEveryone.Win32;

namespace MicaForEveryone.ViewModels
{
    internal class TrayIconViewModel : BaseViewModel, ITrayIconViewModel
    {
        private BackdropType _backdropType;
        private TitlebarColorMode _titlebarColor;
        private bool _extendFrameIntoClientArea;
        private MainWindow _window;

        public TrayIconViewModel()
        {
            ReloadConfigCommand = new RelyCommand(ReloadConfig);
            ChangeTitlebarColorModeCommand = new RelyCommand(ChangeTitlebarColorMode);
            ChangeBackdropTypeCommand = new RelyCommand(ChangeBackdropType);
            ExitCommand = new RelyCommand(Exit);
            EditConfigCommand = new RelyCommand(OpenConfigInEditor);
            OpenSettingsCommand = new RelyCommand(OpenSettings);

            PropertyChanged += TrayIconViewModel_PropertyChanged;
        }

        public bool SystemBackdropIsSupported { get; } =
#if !DEBUG
            SystemBackdrop.IsSupported;
#else
            true;
#endif

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

        public async void InitializeApp(object sender)
        {
            _window = (MainWindow)sender;
            _window.View.ActualThemeChanged += View_ActualThemeChanged;

            var configService = Program.CurrentApp.Container.GetService<IConfigService>();
            await configService.LoadAsync();

            UpdateData();

            await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var ruleService = Program.CurrentApp.Container.GetService<IRuleService>();
                var viewService = Program.CurrentApp.Container.GetService<IViewService>();
                var eventHookService = Program.CurrentApp.Container.GetService<IEventHookService>();

                ruleService.SystemTitlebarColorMode = viewService.SystemColorMode;
                eventHookService.Start();
                ruleService.MatchAndApplyRuleToAllWindows();
            });
        }

        public async void SaveConfig()
        {
            var configService = Program.CurrentApp.Container.GetService<IConfigService>();

            await configService.SaveAsync();
        }

        public async void RematchRules()
        {
            await Task.Run(() =>
            {
                var ruleService = Program.CurrentApp.Container.GetService<IRuleService>();
                ruleService.MatchAndApplyRuleToAllWindows();
            });
        }

        public async void ReloadConfig()
        {
            try
            {
                var configService = Program.CurrentApp.Container.GetService<IConfigService>();
                await configService.LoadAsync();
            }
            catch (ParserError error)
            {
                await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
                    dialogService.ShowErrorDialog(_window, error.Message, error.ToString(), 576, 400);
                });
            }

            UpdateData();
            RematchRules();
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

                _window.Handle.SetWindowPos(
                    HWND.NULL,
                    notifyIconRect,
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                _window.Interop.WindowHandle.SetWindowPos(
                    HWND.NULL,
                    new RECT(0, 0, notifyIconRect.Width, notifyIconRect.Height),
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                menu.ShowAt(_window.View,
                    new Windows.Foundation.Point(
                        (offset.X - notifyIconRect.X) / _window.ScaleFactor,
                        (offset.Y - notifyIconRect.Y) / _window.ScaleFactor));
            }
        }

        public void ShowTipPopup(Rectangle notifyIconRect)
        {
            _window.Handle.SetWindowPos(
                    HWND.NULL,
                    notifyIconRect,
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            _window.Interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, notifyIconRect.Width, notifyIconRect.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            var tooltip = (ToolTip)ToolTipService.GetToolTip(_window.View);
            tooltip.IsOpen = true;
        }

        public void HideTipPopup()
        {
            var tooltip = (ToolTip)ToolTipService.GetToolTip(_window.View);
            tooltip.IsOpen = false;
        }

        private void TrayIconViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var rule = GetGlobalRule();
            switch (e.PropertyName)
            {
                case nameof(BackdropType):
                    rule.BackdropPreference = BackdropType;
                    break;
                case nameof(TitlebarColor):
                    rule.TitlebarColor = TitlebarColor;
                    break;
                case nameof(ExtendFrameIntoClientArea):
                    rule.ExtendFrameIntoClientArea = ExtendFrameIntoClientArea;
                    break;
                default:
                    return;
            }
            _window.RequestRematchRules();
            _window.RequestSaveConfig();
        }

        private async void UpdateData()
        {
            await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var globalRule = GetGlobalRule();
                BackdropType = globalRule.BackdropPreference;
                TitlebarColor = globalRule.TitlebarColor;
                ExtendFrameIntoClientArea = globalRule.ExtendFrameIntoClientArea;
            });
        }

        private IRule GetGlobalRule()
        {
            var configService = Program.CurrentApp.Container.GetService<IConfigService>();
            return configService.Rules.First(rule => rule is GlobalRule);
        }

        private void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            var ruleService = Program.CurrentApp.Container.GetService<IRuleService>();
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            ruleService.SystemTitlebarColorMode = viewService.SystemColorMode;
            _window.RequestRematchRules();
        }

        // commands

        private void ReloadConfig(object parameter)
        {
            _window.RequestReloadConfig();
        }

        private void ChangeTitlebarColorMode(object parameter)
        {
            TitlebarColor = parameter.ToString() switch
            {
                "Default" => TitlebarColorMode.Default,
                "System" => TitlebarColorMode.System,
                "Light" => TitlebarColorMode.Light,
                "Dark" => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
        }

        private void ChangeBackdropType(object parameter)
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
        }

        private void Exit(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.MainWindow.Close();
        }

        private async void OpenConfigInEditor(object obj)
        {
            await Task.Run(() =>
            {
                var configService = Program.CurrentApp.Container.GetService<IConfigService>();
                configService.ConfigSource.OpenInEditor();
            });
        }

        private void OpenSettings(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.ShowSettingsWindow();
        }
    }
}
