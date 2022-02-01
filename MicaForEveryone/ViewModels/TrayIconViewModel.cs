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
        private readonly IConfigService _configService;

        private BackdropType _backdropType;
        private TitlebarColorMode _titlebarColor;
        private bool _extendFrameIntoClientArea;
        private MainWindow _window;
        private IRule _globalRule;

        public TrayIconViewModel(IConfigService configService)
        {
            _configService = configService;

            ReloadConfigCommand = new RelyCommand(ReloadConfig);
            ChangeTitlebarColorModeCommand = new RelyCommand(ChangeTitlebarColorMode);
            ChangeBackdropTypeCommand = new RelyCommand(ChangeBackdropType);
            ExitCommand = new RelyCommand(Exit);
            EditConfigCommand = new RelyCommand(OpenConfigInEditor);
            OpenSettingsCommand = new RelyCommand(OpenSettings);

            _configService.Updated += ConfigService_Changed;
        }

        ~TrayIconViewModel()
        {
            _configService.Updated -= ConfigService_Changed;
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

        public async void Initialize(object sender)
        {
            _window = (MainWindow)sender;
            _window.View.ActualThemeChanged += View_ActualThemeChanged;

            var configService = Program.CurrentApp.Container.GetService<IConfigService>();
            await configService.LoadAsync();

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

        public async Task ReloadConfig()
        {
            try
            {
                await _configService.LoadAsync();
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

        public void ShowTooltipPopup(Rectangle notifyIconRect)
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

        public void HideTooltipPopup()
        {
            var tooltip = (ToolTip)ToolTipService.GetToolTip(_window.View);
            tooltip.IsOpen = false;
        }

        private async Task UpdateDataAsync()
        {
            _globalRule = _configService.Rules.First(rule => rule is GlobalRule);

            await _window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BackdropType = _globalRule.BackdropPreference;
                TitlebarColor = _globalRule.TitlebarColor;
                ExtendFrameIntoClientArea = _globalRule.ExtendFrameIntoClientArea;
            });
        }

        private async Task UpdateRuleAsync()
        {
            _globalRule.BackdropPreference = BackdropType;
            _globalRule.TitlebarColor = TitlebarColor;
            _globalRule.ExtendFrameIntoClientArea = ExtendFrameIntoClientArea;

            _configService.RaiseChanged();
            await _configService.SaveAsync();
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

        private async void ConfigService_Changed(object sender, EventArgs e)
        {
            await UpdateDataAsync();
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

        private async void OpenConfigInEditor(object obj)
        {
            await Task.Run(() =>
            {
                _configService.ConfigSource.OpenInEditor();
            });
        }

        private void OpenSettings(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.ShowSettingsWindow();
        }
    }
}
