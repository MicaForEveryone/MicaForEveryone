using System;

using MicaForEveryone.Models;
using MicaForEveryone.UWP;
using MicaForEveryone.ViewModels;

namespace MicaForEveryone
{
    internal partial class App
    {
        private void InitializeViewModel()
        {
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
            viewModel.ChangeExtendFrameIntoClientAreaCommand = new RelyCommand(ViewModel_ChangeExtendFrameIntoClientArea);
            viewModel.AboutCommand = new RelyCommand(ViewModel_About);
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
            _ruleHandler.SaveConfig();
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
            _ruleHandler.SaveConfig();
            _mainWindow.RequestRematchRules();
        }

        private void ViewModel_ChangeExtendFrameIntoClientArea(object parameter)
        {
            _ruleHandler.GlobalRule.ExtendFrameIntoClientArea = parameter switch
            {
                "True" => true,
                "False" => false,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            UpdateViewModel();
            _ruleHandler.SaveConfig();
            _mainWindow.RequestRematchRules();
        }

        private void ViewModel_About(object obj)
        {
            ShowAboutDialog();
        }
    }
}