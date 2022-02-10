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

        private CoreDispatcher? _dispatcher;
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

        public void Initialize(object sender)
        {
            if (sender is FrameworkElement element)
            {
                _dispatcher = element.Dispatcher;
            }

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

        private void SettingsService_Changed(object? sender, SettingsChangedEventArgs args)
        {
            _dispatcher!.RunAsync(CoreDispatcherPriority.Normal, () =>
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
                        PaneItems.RemoveAt(index+1);
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
            }).AsTask().Start();
        }

        // commands

        private void Close(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService?.SettingsWindow?.Close();
        }

        private void AddProcessRule(object obj)
        {
            var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();

            AddProcessRuleDialog dialog = new();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += (sender, args) =>
            {
                _newRule = new ProcessRule(dialog.ViewModel.ProcessName);
                _settingsService.RaiseChanged(SettingsChangeType.RuleAdded, _newRule);
            };

            dialogService?.ShowDialog(viewService?.SettingsWindow, dialog);
        }

        private void AddClassRule(object obj)
        {
            var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();

            AddClassRuleDialog dialog = new();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += (sender, args) =>
            {
                _newRule = new ClassRule(dialog.ViewModel.ClassName);
                _settingsService.RaiseChanged(SettingsChangeType.RuleAdded, _newRule);
            };

            dialogService?.ShowDialog(viewService?.SettingsWindow, dialog);
        }

        private void RemoveRule(object obj)
        {
            if (SelectedPane is RulePaneItem rulePane)
            {
                if (rulePane.ViewModel.Rule is IRule rule)
                {
                    _settingsService.RaiseChanged(SettingsChangeType.RuleRemoved, rule);
                    SelectedPane = _generalPane;
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
                var window = Program.CurrentApp.Container.GetService<IViewService>()?.SettingsWindow;
                await window?.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var dialogService = Program.CurrentApp.Container.GetService<IDialogService>();
                    dialogService?.ShowErrorDialog(window, error.Message, error.ToString(), 576, 400);
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
    }
}
