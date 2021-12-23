using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using MicaForEveryone.Views;

namespace MicaForEveryone
{
    internal partial class App
    {
        private readonly MainWindow _mainWindow = new();

        private void InitializeMainWindow()
        {
            _mainWindow.ReloadConfigRequested += MainWindow_ReloadConfigRequested;
            _mainWindow.RematchRulesRequested += MainWindow_RematchRulesRequested;
            _mainWindow.View.ActualThemeChanged += MainWindow_ThemeChanged;
            _mainWindow.Activate();
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

        private void MainWindow_ThemeChanged(FrameworkElement sender, object args)
        {
            SetSystemColorMode();
            _mainWindow.RequestRematchRules();
        }
    }
}