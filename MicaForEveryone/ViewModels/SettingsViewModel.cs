using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Windows.UI.Xaml;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Views;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : BaseViewModel, ISettingsViewModel
    {
        private readonly ISettingsService _settingsService;

        private SettingsWindow? _window;
        private IPaneItem? _selectedPane;
        private GeneralPaneItem _generalPane;
        private IRule? _newRule;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settingsService.Changed += SettingsService_Changed;
            _generalPane = new GeneralPaneItem(Program.CurrentApp.Container.GetService<IGeneralSettingsViewModel>());

            CloseCommand = new RelyCommand(Close);
            AddProcessRuleCommand = new RelyCommand(AddProcessRule);
            AddClassRuleCommand = new RelyCommand(AddClassRule);
            RemoveRuleCommand = new RelyCommand(RemoveRule, CanRemoveRule);
            ReloadConfigCommand = new RelyCommand(ReloadConfig);
            EditConfigCommand = new RelyCommand(OpenConfigInEditor);
            ResetConfigCommand = new RelyCommand(ResetConfig);
        }

        ~SettingsViewModel()
        {
            _settingsService.Changed -= SettingsService_Changed;
        }

        public bool IsBackdropSupported =>
            DesktopWindowManager.IsBackdropTypeSupported;

        public bool IsMicaSupported =>
            DesktopWindowManager.IsUndocumentedMicaSupported || DesktopWindowManager.IsBackdropTypeSupported;

        public bool IsImmersiveDarkModeSupported =>
            DesktopWindowManager.IsImmersiveDarkModeSupported;

        public Version Version { get; } = typeof(Program).Assembly.GetName().Version!;

        public ObservableCollection<IPaneItem> PaneItems { get; set; } = new();

        public IPaneItem? SelectedPane
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
        public ICommand ResetConfigCommand { get; }

        public void Initialize(object sender)
        {
            _window = sender as SettingsWindow;
            _generalPane.ViewModel.Initialize(_window);

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

        private async void SettingsService_Changed(object? sender, SettingsChangedEventArgs args)
        {
            await _window?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

        private void Close(object obj)
        {
            _window?.Close();
        }

        private void AddProcessRule(object obj)
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

        private void AddClassRule(object obj)
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

        private async void RemoveRule(object obj)
        {
            if (SelectedPane is RulePaneItem rulePane)
            {
                if (rulePane.ViewModel.Rule is IRule rule)
                {
                    SelectedPane = _generalPane;
                    await _settingsService.CommitChangesAsync(SettingsChangeType.RuleRemoved, rule);
                }
            }
        }

        private bool CanRemoveRule(object obj) => SelectedPane != null &&
            SelectedPane.ItemType is not (PaneItemType.General or PaneItemType.Global);

        private async void ReloadConfig(object parameter)
        {
            try
            {
                await _settingsService.LoadRulesAsync();
            }
            catch (ParserError error)
            {
                await _window?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
                    dialogService?.ShowErrorDialog(_window, error.Message, error.ToString(), 576, 400);
                });
            }
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

        private async void ResetConfig(object parameter)
        {
            await _settingsService.ConfigFile.ResetAsync();
            await _settingsService.LoadRulesAsync();
        }
    }
}
