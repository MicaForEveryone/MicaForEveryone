using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI.Models;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly GeneralPaneItem _generalPane;

        private SettingsWindow? _window;
        private IPaneItem? _selectedPane;
        private IRule? _newRule;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settingsService.Changed += SettingsService_Changed;

            var vmGeneralPane = Program.CurrentApp.Container.GetService<IGeneralSettingsViewModel>();
            _generalPane = new GeneralPaneItem(vmGeneralPane);

            CloseCommand = new RelayCommand(DoClose);
            AddProcessRuleCommand = new RelayCommand(DoAddProcessRule);
            AddClassRuleCommand = new RelayCommand(DoAddClassRule);
            RemoveRuleAsyncCommand = new AsyncRelayCommand(DoRemoveRuleAsync, CanRemoveRule);

            if (Application.IsPackaged)
            {
                Version = Package.Current.Id.Version.ToString() ?? "<unknown>";
            }
            else
            {
                Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "<unknown>";
            }
        }

        ~SettingsViewModel()
        {
            _settingsService.Changed -= SettingsService_Changed;
        }

        // properties

        public bool IsBackdropSupported => DesktopWindowManager.IsBackdropTypeSupported;
        public bool IsMicaSupported => DesktopWindowManager.IsUndocumentedMicaSupported;
        public bool IsImmersiveDarkModeSupported => DesktopWindowManager.IsImmersiveDarkModeSupported;

        public string Version { get; }

        public IList<BackdropType> BackdropTypes { get; } = new List<BackdropType>();
        public IList<TitlebarColorMode> TitlebarColorModes { get; } = new List<TitlebarColorMode>();

        public IList<IPaneItem> PaneItems { get; set; } = new ObservableCollection<IPaneItem>();

        public IPaneItem? SelectedPane
        {
            get => _selectedPane;
            set
            {
                SetProperty(ref _selectedPane, value);
                RemoveRuleAsyncCommand.NotifyCanExecuteChanged();
            }
        }

        public ICommand CloseCommand { get; }
        public ICommand AddProcessRuleCommand { get; }
        public ICommand AddClassRuleCommand { get; }
        public IAsyncRelayCommand RemoveRuleAsyncCommand { get; }

        // public methods

        public void Initialize(SettingsWindow sender)
        {
            _window = sender;

            if (_generalPane.ViewModel is IGeneralSettingsViewModel vmGeneralPane)
                vmGeneralPane.Initialize(sender);

            if (BackdropTypes.Count <= 0)
            {
                BackdropTypes.Add(BackdropType.Default);
                if (IsMicaSupported)
                {
                    BackdropTypes.Add(BackdropType.None);
                    BackdropTypes.Add(BackdropType.Mica);
                }
                if (IsBackdropSupported)
                {
                    BackdropTypes.Add(BackdropType.Acrylic);
                    BackdropTypes.Add(BackdropType.Tabbed);
                }
            }

            if (TitlebarColorModes.Count <= 0)
            {
                TitlebarColorModes.Add(TitlebarColorMode.Default);
                if (IsImmersiveDarkModeSupported)
                {
                    TitlebarColorModes.Add(TitlebarColorMode.System);
                    TitlebarColorModes.Add(TitlebarColorMode.Light);
                    TitlebarColorModes.Add(TitlebarColorMode.Dark);
                }
            }

            PopulatePanes();
        }

        // helper

        private void PopulatePanes()
        {
            PaneItems.Add(_generalPane);
            SelectedPane = _generalPane;

            foreach (var rule in _settingsService.Rules)
            {
                var item = rule.GetPaneItem(this);
                item.ViewModel.ParentViewModel = this;
                PaneItems.Add(item);
            }
        }

        // event handlers

        private void SettingsService_Changed(object? sender, SettingsChangedEventArgs args)
        {
            Program.CurrentApp.Dispatcher.Enqueue(() =>
            {
                var pane = args.Rule?.GetPaneItem(this);
                var lastPane = SelectedPane;

                switch (args.Type)
                {
                    case SettingsChangeType.RuleAdded:
                        PaneItems.Add(pane!);
                        if (args.Rule == _newRule)
                        {
                            SelectedPane = pane;
                            _newRule = null;
                        }
                        break;

                    case SettingsChangeType.RuleRemoved:
                        PaneItems.Remove(pane!);
                        if (args.Rule == _newRule)
                        {
                            SelectedPane = pane;
                            _newRule = null;
                        }
                        break;

                    case SettingsChangeType.RuleChanged:
                        var index = PaneItems.IndexOf(pane!);
                        PaneItems.Insert(index, pane!);
                        PaneItems.RemoveAt(index + 1);
                        if (lastPane?.Equals(pane) ?? false)
                            SelectedPane = pane;
                        break;

                    case SettingsChangeType.ConfigFileReloaded:
                        SelectedPane = null;
                        PaneItems.Clear();
                        PopulatePanes();

                        // return to last pane if it's still there
                        lastPane = PaneItems.FirstOrDefault(item => item.Equals(lastPane));
                        if (lastPane != null)
                            SelectedPane = lastPane;
                        break;
                }
            });
        }

        // commands 

        private void DoClose()
        {
            _window?.Close();
        }

        private void DoAddProcessRule()
        {
            var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();

            AddProcessRuleDialog dialog = new();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += async (sender, args) =>
            {
                _newRule = new ProcessRule(dialog.ViewModel.ProcessName);
                await _settingsService.CommitChangesAsync(SettingsChangeType.RuleAdded, _newRule);
            };

            dialogService?.ShowDialog(_window, dialog);
        }

        private void DoAddClassRule()
        {
            var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();

            AddClassRuleDialog dialog = new();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += async (sender, args) =>
            {
                _newRule = new ClassRule(dialog.ViewModel.ClassName);
                await _settingsService.CommitChangesAsync(SettingsChangeType.RuleAdded, _newRule);
            };

            dialogService?.ShowDialog(_window, dialog);
        }

        private async Task DoRemoveRuleAsync()
        {
            if (SelectedPane is RulePaneItem rulePane &&
                    rulePane.ViewModel is IRuleSettingsViewModel viewModel)
            {
                SelectedPane = _generalPane;
                await _settingsService.CommitChangesAsync(SettingsChangeType.RuleRemoved, viewModel.Rule);
            }
        }

        private bool CanRemoveRule() => SelectedPane != null &&
            SelectedPane.ItemType is not (PaneItemType.General or PaneItemType.Global);
    }
}
