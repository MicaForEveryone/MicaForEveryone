using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Config;

#if !DEBUG
using MicaForEveryone.Win32;
#endif

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : BaseViewModel, ISettingsViewModel
    {
        private readonly IConfigService _configService;

        private CoreDispatcher _dispatcher;
        private IPaneItem _selectedPane;

        public SettingsViewModel(IConfigService configService)
        {
            _configService = configService;
            _configService.Updated += ConfigService_Changed;

            CloseCommand = new RelyCommand(Close);
            AddProcessRuleCommand = new RelyCommand(AddProcessRule);
            AddClassRuleCommand = new RelyCommand(AddClassRule);
            RemoveRuleCommand = new RelyCommand(RemoveRule, CanRemoveRule);
            ReloadConfigCommand = new RelyCommand(ReloadConfig);
            EditConfigCommand = new RelyCommand(OpenConfigInEditor);
        }

        ~SettingsViewModel()
        {
            _configService.Updated -= ConfigService_Changed;
        }

        public bool SystemBackdropIsSupported =>
#if !DEBUG
            DesktopWindowManager.IsBackdropTypeSupported;
#else
            true;
#endif

        public Version Version { get; } = typeof(Program).Assembly.GetName().Version;

        public ObservableCollection<IPaneItem> PaneItems { get; set; } = new();
        public IPaneItem SelectedPane
        {
            get => _selectedPane;
            set
            {
                SetProperty(ref _selectedPane, value);
                ((RelyCommand)RemoveRuleCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<BackdropType> BackdropTypes { get; } = new();
        public ObservableCollection<TitlebarColorMode> TitlebarColorModes { get; } = new();

        public ICommand CloseCommand { get; }
        public ICommand AddProcessRuleCommand { get; }
        public ICommand AddClassRuleCommand { get; }
        public ICommand RemoveRuleCommand { get; }
        public ICommand EditConfigCommand { get; }
        public ICommand ReloadConfigCommand { get; }

        public void Initialize(object sender)
        {
            if (sender is FrameworkElement element)
            {
                _dispatcher = element.Dispatcher;
            }

            if (BackdropTypes.Count <= 0)
            {
                BackdropTypes.Add(BackdropType.Default);
                BackdropTypes.Add(BackdropType.None);
                BackdropTypes.Add(BackdropType.Mica);
                if (SystemBackdropIsSupported)
                {
                    BackdropTypes.Add(BackdropType.Acrylic);
                    BackdropTypes.Add(BackdropType.Tabbed);
                }
            }

            if (TitlebarColorModes.Count <= 0)
            {
                TitlebarColorModes.Add(TitlebarColorMode.Default);
                TitlebarColorModes.Add(TitlebarColorMode.System);
                TitlebarColorModes.Add(TitlebarColorMode.Light);
                TitlebarColorModes.Add(TitlebarColorMode.Dark);
            }

            PopulatePanes();
        }

        private void PopulatePanes()
        {
            var generalPane = new GeneralPaneItem(
                Program.CurrentApp.Container.GetService<IGeneralSettingsViewModel>());
            PaneItems.Add(generalPane);
            SelectedPane = generalPane;

            foreach (var rule in _configService.Rules)
            {
                var item = rule.GetPaneItem(this);
                item.ViewModel.ParentViewModel = this;
                PaneItems.Add(item);
            }
        }

        private async void ConfigService_Changed(object sender, EventArgs e)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // save current pane
                var lastPane = SelectedPane;

                SelectedPane = null;
                PaneItems.Clear();
                PopulatePanes();

                // return to last pane if it's still there
                lastPane = PaneItems.FirstOrDefault(item => item.Equals(lastPane));
                if (lastPane != null)
                {
                    SelectedPane = lastPane;
                }
            });
        }

        // commands

        private void Close(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.SettingsWindow?.Close();
        }

        private void AddProcessRule(object obj)
        {
            var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            
            var dialog = new Views.AddProcessRuleDialog();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += async (sender, args) =>
            {
                var rule = new ProcessRule(dialog.ViewModel.ProcessName);
                _configService.ConfigSource.SetRule(rule);
                await _configService.ConfigSource.SaveAsync();
                _configService.PopulateRules();
            };

            dialogService.ShowDialog(viewService.SettingsWindow, dialog);
        }

        private void AddClassRule(object obj)
        {
            var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            
            var dialog = new Views.AddClassRuleDialog();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += async (sender, args) =>
            {
                var rule = new ClassRule(dialog.ViewModel.ClassName);
                _configService.ConfigSource.SetRule(rule);
                await _configService.ConfigSource.SaveAsync();
                _configService.PopulateRules();
            };

            dialogService.ShowDialog(viewService.SettingsWindow, dialog);
        }

        private async void RemoveRule(object obj)
        {
            if (SelectedPane is RulePaneItem rulePane)
            {
                if (rulePane.ViewModel.Rule is IRule rule)
                {
                    _configService.ConfigSource.RemoveRule(rule);
                    await _configService.ConfigSource.SaveAsync();
                    _configService.PopulateRules();
                }
            }
        }

        private bool CanRemoveRule(object obj) => SelectedPane != null &&
            SelectedPane.ItemType is not (PaneItemType.General or PaneItemType.Global);

        private async void ReloadConfig(object parameter)
        {
            try
            {
                await _configService.LoadAsync();
            }
            catch (ParserError error)
            {
                var window = Program.CurrentApp.Container.GetService<IViewService>().SettingsWindow;
                await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
                    dialogService.ShowErrorDialog(window, error.Message, error.ToString(), 576, 400);
                });
            }
        }

        private async void OpenConfigInEditor(object obj)
        {
            await Task.Run(() =>
            {
                _configService.ConfigSource.OpenInEditor();
            });
        }
    }
}
