using System;
using Vanara.PInvoke;

using MicaForEveryone.Rules;
using MicaForEveryone.UWP;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;
using MicaForEveryone.ViewModels;

namespace MicaForEveryone
{
    internal class App : XamlApplication, IDisposable
    {
        private readonly RuleHandler _ruleHandler = new();
        private readonly WinEventHook _eventHook = new(User32.EventConstants.EVENT_OBJECT_CREATE);
        private readonly UWP.App _uwpApp = new();

        private MainWindow _mainWindow;

        public App()
        {
            BeforeRun += App_BeforeRun;
        }

        private void App_BeforeRun(object sender, EventArgs e)
        {
            // load config file
            var args = Environment.GetCommandLineArgs();
            var filePath = args.Length > 1 ? args[1] : "config.ini";
            _ruleHandler.ConfigSource = new ConfigFileReader(filePath);
            _ruleHandler.LoadConfig();
            // initialize view model
            var viewModel = (_mainWindow.View as MainWindowView).ViewModel;
            #if DEBUG
            viewModel.SystemBackdropIsSupported = true;
            #else
            viewModel.SystemBackdropIsSupported = SystemBackdrop.IsSupported;
            #endif
            viewModel.Exit = new RelyCommand(_ => _mainWindow.Close());
            viewModel.ReloadConfig = new RelyCommand(ViewModel_ReloadConfig);
            viewModel.ChangeTitlebarColorMode = new RelyCommand(ViewModel_ChangeTitlebarColorMode);
            viewModel.ChangeBackdropType = new RelyCommand(ViewModel_ChangeBackdropType);
            viewModel.ToggleExtendFrameIntoClientArea = new RelyCommand(ViewModel_ToggleExtendFrameIntoClientArea);
            UpdateViewModel();
            // apply rules to open windows
            _ruleHandler.MatchAndApplyRuleToAllWindows();
            // hook event new window event to apply rules on new window
            _eventHook.HookTriggered += EventHook_Triggered;
            _eventHook.Hook();
        }

        public void Run()
        {
            _mainWindow = new();
            Run(_mainWindow);
        }

        public void Dispose()
        {
            _mainWindow.Dispose();
            _eventHook.Dispose();
            _uwpApp.Dispose();
        }

        private void EventHook_Triggered(object sender, HookTriggeredEventArgs e)
        {
            _ruleHandler.MatchAndApplyRuleToWindow(e.WindowHandle);
        }

        private void UpdateViewModel()
        {
            var viewModel = (_mainWindow.View as MainWindowView).ViewModel;
            viewModel.BackdropType = _ruleHandler.GlobalRule.BackdropPreference;
            viewModel.TitlebarColor = _ruleHandler.GlobalRule.TitlebarColor;
            viewModel.ExtendFrameIntoClientArea = _ruleHandler.GlobalRule.ExtendFrameIntoClientArea;
        }

        private void ViewModel_ReloadConfig(object parameter)
        {
            _ruleHandler.ConfigSource.Reload();
            _ruleHandler.LoadConfig();
            _ruleHandler.MatchAndApplyRuleToAllWindows();
            UpdateViewModel();
        }

        private void ViewModel_ChangeTitlebarColorMode(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Default":
                    _ruleHandler.GlobalRule.TitlebarColor = Models.TitlebarColorMode.Default;
                    break;
                case "System":
                    _ruleHandler.GlobalRule.TitlebarColor = Models.TitlebarColorMode.System;
                    break;
                case "Light":
                    _ruleHandler.GlobalRule.TitlebarColor = Models.TitlebarColorMode.Light;
                    break;
                case "Dark":
                    _ruleHandler.GlobalRule.TitlebarColor = Models.TitlebarColorMode.Dark;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter));
            }
            _ruleHandler.MatchAndApplyRuleToAllWindows();
            UpdateViewModel();
        }

        private void ViewModel_ChangeBackdropType(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Default":
                    _ruleHandler.GlobalRule.BackdropPreference = Models.BackdropType.Default;
                    break;
                case "None":
                    _ruleHandler.GlobalRule.BackdropPreference = Models.BackdropType.None;
                    break;
                case "Mica":
                    _ruleHandler.GlobalRule.BackdropPreference = Models.BackdropType.Mica;
                    break;
                case "Acrylic":
                    _ruleHandler.GlobalRule.BackdropPreference = Models.BackdropType.Acrylic;
                    break;
                case "Tabbed":
                    _ruleHandler.GlobalRule.BackdropPreference = Models.BackdropType.Tabbed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter));
            }
            _ruleHandler.MatchAndApplyRuleToAllWindows();
            UpdateViewModel();
        }

        private void ViewModel_ToggleExtendFrameIntoClientArea(object parameter)
        {
            _ruleHandler.GlobalRule.ExtendFrameIntoClientArea = !_ruleHandler.GlobalRule.ExtendFrameIntoClientArea;
            _ruleHandler.MatchAndApplyRuleToAllWindows();
            UpdateViewModel();
        }
    }
}
