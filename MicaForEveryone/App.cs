using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Vanara.PInvoke;

using MicaForEveryone.Rules;
using MicaForEveryone.UWP;
using MicaForEveryone.Models;
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
        private readonly MainWindow _mainWindow = new();

        public App()
        {
            _mainWindow.ReloadConfigRequested += MainWindow_ReloadConfigRequested;
            _mainWindow.RematchRulesRequested += MainWindow_RematchRulesRequested;
        }

        private async void MainWindow_RematchRulesRequested(object sender, EventArgs e)
        {
            await Task.Run(() => _ruleHandler.MatchAndApplyRuleToAllWindows());
        }

        private async void MainWindow_ReloadConfigRequested(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                _ruleHandler.ConfigSource.Reload();
                _ruleHandler.LoadConfig();
                _ruleHandler.MatchAndApplyRuleToAllWindows();
                UpdateViewModel();
            });
        }

        public void Run()
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                var errorContent = new ContentDialogView
                {
                    ViewModel =
                    {
                        Title = "Error",
                        Content = "Mica for Everyone at least requires Windows 11 (10.0.22000) to work.",
                        IsPrimaryButtonEnabled = true,
                        PrimaryButtonContent = "OK",
                    },
                };
                using var errorDialog = new XamlDialog(errorContent)
                {
                    ClassName = "Dialog",
                    Title = "Mica For Everyone",
                    Width = 576,
                    Height = 320,
                    Style = User32.WindowStyles.WS_DLGFRAME,
                };
                errorContent.ViewModel.PrimaryCommand = new RelyCommand(_ =>
                {
                    errorDialog.Close();
                    Exit();
                });
                errorDialog.CenterToDesktop();
                errorDialog.Activate();
                errorDialog.Show();
                Run(errorDialog);
                return;
            }

            _mainWindow.Activate();

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
            viewModel.ExitCommand = new RelyCommand(_ => _mainWindow.Close());
            viewModel.ReloadConfigCommand = new RelyCommand(ViewModel_ReloadConfig);
            viewModel.ChangeTitlebarColorModeCommand = new RelyCommand(ViewModel_ChangeTitlebarColorMode);
            viewModel.ChangeBackdropTypeCommand = new RelyCommand(ViewModel_ChangeBackdropType);
            viewModel.ToggleExtendFrameIntoClientAreaCommand = new RelyCommand(ViewModel_ToggleExtendFrameIntoClientArea);
            viewModel.AboutCommand = new RelyCommand(ViewModel_About);
            UpdateViewModel();
            // find system mode
            _ruleHandler.SystemTitlebarMode = _uwpApp.RequestedTheme switch
            {
                ApplicationTheme.Light => TitlebarColorMode.Light,
                ApplicationTheme.Dark => TitlebarColorMode.Dark,
                _ => TitlebarColorMode.Default,
            };
            // apply rules to open windows
            _ruleHandler.MatchAndApplyRuleToAllWindows();
            // hook event new window event to apply rules on new window
            _eventHook.HookTriggered += EventHook_Triggered;
            _eventHook.Hook();

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
            _mainWindow.RequestReloadConfig();
        }

        private void ViewModel_ChangeTitlebarColorMode(object parameter)
        {
            _ruleHandler.GlobalRule.TitlebarColor = parameter.ToString() switch
            {
                "Default" => TitlebarColorMode.Default,
                "System" => TitlebarColorMode.System,
                "Light" => TitlebarColorMode.Light,
                "Dark" => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            UpdateViewModel();
            _mainWindow.RequestRematchRules();
        }

        private void ViewModel_ChangeBackdropType(object parameter)
        {
            _ruleHandler.GlobalRule.BackdropPreference = parameter.ToString() switch
            {
                "Default" => BackdropType.Default,
                "None" => BackdropType.None,
                "Mica" => BackdropType.Mica,
                "Acrylic" => BackdropType.Acrylic,
                "Tabbed" => BackdropType.Tabbed,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            UpdateViewModel();
            _mainWindow.RequestRematchRules();
        }

        private void ViewModel_ToggleExtendFrameIntoClientArea(object parameter)
        {
            _ruleHandler.GlobalRule.ExtendFrameIntoClientArea = !_ruleHandler.GlobalRule.ExtendFrameIntoClientArea;
            UpdateViewModel();
            _mainWindow.RequestRematchRules();
        }

        private void ViewModel_About(object obj)
        {
            var openUrlCommand = new RelyCommand(async url => 
                await Windows.System.Launcher.LaunchUriAsync((Uri)url));
            var view = new ContentDialogView
            {
                ViewModel =
                {
                    Title = "Mica For Everyone",
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock { Text = "v" + typeof(App).Assembly.GetName().Version},
                            new HyperlinkButton { 
                                Content = "Github",
                                Command = openUrlCommand,
                                CommandParameter = new Uri("https://github.com/minusium/MicaForEveryone"),
                            },
                        },
                        Spacing = 5,
                    },
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonContent = "Close",
                },
            };
            var xamlDialog = new XamlDialog(view)
            {
                ClassName = "Dialog",
                Parent = _mainWindow.Handle,
                Title = "About",
                Width = 400,
                Height = 600,
            };
            view.ViewModel.PrimaryCommand = new RelyCommand(_ =>
            {
                xamlDialog.Close();
            });
            xamlDialog.CenterToDesktop();
            xamlDialog.Activate();
            xamlDialog.Show();
            User32.SetForegroundWindow(xamlDialog.Handle);
        }
    }
}
