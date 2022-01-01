using System;
using System.Linq;
using System.Threading.Tasks;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Views;

namespace MicaForEveryone.Services
{
    public class ViewModel : IViewModel
    {
        private readonly IRuleService _ruleService;
        private readonly IConfigService _configService;
        private readonly IDialogService _dialogService;
        private readonly IEventHookService _eventHookService;

        private MainWindow _view;
        private MainViewModel _viewModel;

        public ViewModel(
            IRuleService ruleService,
            IConfigService configService,
            IDialogService dialogService,
            IEventHookService eventHookService)
        {
            _ruleService = ruleService;
            _configService = configService;
            _dialogService = dialogService;
            _eventHookService = eventHookService;
        }

        public void Attach(MainWindow view)
        {
            _view = view;
            _viewModel = ((TrayIconView)view.View).ViewModel;

            _view.ReloadConfigRequested += MainWindow_ReloadConfigRequested;
            _view.RematchRulesRequested += MainWindow_RematchRulesRequested;
            _view.SaveConfigRequested += MainWindow_SaveConfigRequested;
            _view.View.Loaded += View_Loaded;

#if DEBUG
            _viewModel.SystemBackdropIsSupported = true;
#else
            _viewModel.SystemBackdropIsSupported = SystemBackdrop.IsSupported;
#endif
            _viewModel.ExitCommand = new RelyCommand(_ => _view.Close());
            _viewModel.ReloadConfigCommand = new RelyCommand(ViewModel_ReloadConfig);
            _viewModel.ChangeTitlebarColorModeCommand = new RelyCommand(ViewModel_ChangeTitlebarColorMode);
            _viewModel.ChangeBackdropTypeCommand = new RelyCommand(ViewModel_ChangeBackdropType);
            _viewModel.ChangeExtendFrameIntoClientAreaCommand = new RelyCommand(ViewModel_ChangeExtendFrameIntoClientArea);
            _viewModel.AboutCommand = new RelyCommand(ViewModel_About);
        }

        private async void View_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await _configService.LoadAsync();
            _eventHookService.Start();
        }

        private async void MainWindow_SaveConfigRequested(object sender, EventArgs e)
        {
            await _configService.SaveAsync();
        }

        private async void MainWindow_RematchRulesRequested(object sender, EventArgs e)
        {
            await Task.Run(() => _ruleService.MatchAndApplyRuleToAllWindows());
        }

        private async void MainWindow_ReloadConfigRequested(object sender, EventArgs e)
        {
            try
            {
                await _configService.LoadAsync();
                UpdateViewModel();
                await Task.Run(() => _ruleService.MatchAndApplyRuleToAllWindows());
            }
            catch (ParserError error)
            {
                await _view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                    () => _dialogService.ShowErrorDialog(_view, error.Message, error.ToString(), 576, 400));
            }
        }

        private async void UpdateViewModel()
        {
            await _view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var globalRule = _configService.Rules.First(rule => rule is GlobalRule);
                _viewModel.BackdropType = globalRule.BackdropPreference;
                _viewModel.TitlebarColor = globalRule.TitlebarColor;
                _viewModel.ExtendFrameIntoClientArea = globalRule.ExtendFrameIntoClientArea;
            });
        }

        private void ViewModel_ReloadConfig(object parameter)
        {
            _view.RequestReloadConfig();
        }

        private void ViewModel_ChangeTitlebarColorMode(object parameter)
        {
            var globalRule = _configService.Rules.First(rule => rule is GlobalRule);
            globalRule.TitlebarColor = parameter.ToString() switch
            {
                "Default" => TitlebarColorMode.Default,
                "System" => TitlebarColorMode.System,
                "Light" => TitlebarColorMode.Light,
                "Dark" => TitlebarColorMode.Dark,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            UpdateViewModel();
            _view.RequestRematchRules();
            _view.RequestSaveConfig();
        }

        private void ViewModel_ChangeBackdropType(object parameter)
        {
            var globalRule = _configService.Rules.First(rule => rule is GlobalRule);
            globalRule.BackdropPreference = parameter.ToString() switch
            {
                "Default" => BackdropType.Default,
                "None" => BackdropType.None,
                "Mica" => BackdropType.Mica,
                "Acrylic" => BackdropType.Acrylic,
                "Tabbed" => BackdropType.Tabbed,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            UpdateViewModel();
            _view.RequestRematchRules();
            _view.RequestSaveConfig();
        }

        private void ViewModel_ChangeExtendFrameIntoClientArea(object parameter)
        {
            var globalRule = _configService.Rules.First(rule => rule is GlobalRule);
            globalRule.ExtendFrameIntoClientArea = parameter switch
            {
                "True" => true,
                "False" => false,
                _ => throw new ArgumentOutOfRangeException(nameof(parameter)),
            };
            UpdateViewModel();
            _view.RequestRematchRules();
            _view.RequestSaveConfig();
        }

        private void ViewModel_About(object obj)
        {
            var dialog = new AboutDialog();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            _dialogService.ShowDialog(_view, dialog);
        }
    }
}
