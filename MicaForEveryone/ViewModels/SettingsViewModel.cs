using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Core.Models;
using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Core.Ui.Models;
using MicaForEveryone.Core.Ui.ViewModels;
using MicaForEveryone.Core.Ui.Views;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : ObservableObject, ISettingsViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly ISettingsContainer _settingsContainer;
        private readonly IViewService _viewService;
        private readonly IDialogService _dialogService;

        private readonly GeneralPaneItem _generalPane;

        private ISettingsView? _view;
        private IPaneItem? _selectedPane;
        private IRule? _newRule;

        public SettingsViewModel(ISettingsService settingsService, ISettingsContainer settingsContainer, IViewService viewService, IDialogService dialogService)
        {
            _settingsService = settingsService;
            _settingsContainer = settingsContainer;
            _viewService = viewService;
            _dialogService = dialogService;

            _settingsService.RuleAdded += SettingsService_RuleAdded;
            _settingsService.RuleRemoved += SettingsService_RuleRemoved;
            _settingsService.RuleChanged += SettingsService_RuleChanged;
            _settingsService.ConfigFileReloaded += SettingsService_ConfigReloaded;

            var vmGeneralPane = Program.Container.GetRequiredService<IGeneralSettingsViewModel>();
            _generalPane = new GeneralPaneItem(vmGeneralPane);

            CloseCommand = new RelayCommand(DoClose);
            AddProcessRuleCommand = new RelayCommand(DoAddProcessRule);
            AddClassRuleCommand = new RelayCommand(DoAddClassRule);
            RemoveRuleAsyncCommand = new AsyncRelayCommand(DoRemoveRuleAsync, CanRemoveRule);

            if (Application.IsPackaged)
            {
                var version = Package.Current.Id.Version;
                Version = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
            else
            {
                Version = typeof(Program).Assembly.GetName().Version?.ToString() ?? "<unknown>";
            }
        }

        ~SettingsViewModel()
        {
            _settingsService.RuleAdded -= SettingsService_RuleAdded;
            _settingsService.RuleRemoved -= SettingsService_RuleRemoved;
            _settingsService.RuleChanged -= SettingsService_RuleChanged;
            _settingsService.ConfigFileReloaded -= SettingsService_ConfigReloaded;
        }

        // properties

        public bool IsBackdropSupported => DesktopWindowManager.IsBackdropTypeSupported;
        public bool IsMicaSupported => DesktopWindowManager.IsUndocumentedMicaSupported;
        public bool IsImmersiveDarkModeSupported => DesktopWindowManager.IsImmersiveDarkModeSupported;
        public bool IsCornerPreferenceSupported => DesktopWindowManager.IsCornerPreferenceSupported;

        public string Version { get; }

        public IList<BackdropType> BackdropTypes { get; } = new List<BackdropType>();
        public IList<TitlebarColorMode> TitlebarColorModes { get; } = new List<TitlebarColorMode>();
        public IList<CornerPreference> CornerPreferences { get; } = new List<CornerPreference>();

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

        public void Attach(ISettingsView view)
        {
            _view = view;

            // restore saved WindowPlacement
            if (view is Window window)
            {
                if (_settingsContainer.GetValue("WindowPlacement") is byte[] serialized)
                {
                    using var stream = new MemoryStream(serialized);
                    var serializer = new BinaryFormatter();
                    var placement = (WINDOWPLACEMENT)serializer.Deserialize(stream);
                    window.SetWindowPlacement(placement);
                }

                window.Destroy += OnClose;
            }

            if (_generalPane.ViewModel is IGeneralSettingsViewModel vmGeneralPane)
                vmGeneralPane.Attach(_view);

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

            if (CornerPreferences.Count <= 0)
            {
                CornerPreferences.Add(CornerPreference.Default);
                if (IsCornerPreferenceSupported)
                {
                    CornerPreferences.Add(CornerPreference.Square);
                    CornerPreferences.Add(CornerPreference.Rounded);
                    CornerPreferences.Add(CornerPreference.RoundedSmall);
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
                var item = rule.GetPaneItem(this, Program.Container.GetRequiredService<IRuleSettingsViewModel>());
                item.ViewModel.ParentViewModel = this;
                PaneItems.Add(item);
            }
        }

        // event handlers

        private void SettingsService_RuleAdded(object? sender, RulesChangeEventArgs args)
        {
            _viewService.DispatcherEnqueue(() =>
            {
                var pane = args.Rule.GetPaneItem(this, Program.Container.GetRequiredService<IRuleSettingsViewModel>());
                var lastPane = SelectedPane;

                PaneItems.Add(pane!);
                if (args.Rule == _newRule)
                {
                    SelectedPane = pane;
                    _newRule = null;
                }
            });
        }

        private void SettingsService_RuleRemoved(object? sender, RulesChangeEventArgs args)
        {
            _viewService.DispatcherEnqueue(() =>
            {
                var pane = args.Rule.GetPaneItem(this, Program.Container.GetRequiredService<IRuleSettingsViewModel>());
                var lastPane = SelectedPane;

                PaneItems.Remove(pane!);
                if (args.Rule == _newRule)
                {
                    SelectedPane = pane;
                    _newRule = null;
                }
            });
        }

        private void SettingsService_RuleChanged(object? sender, RulesChangeEventArgs args)
        {
            _viewService.DispatcherEnqueue(() =>
            {
                var pane = args.Rule.GetPaneItem(this, Program.Container.GetRequiredService<IRuleSettingsViewModel>());
                var lastPane = SelectedPane;

                var index = PaneItems.IndexOf(pane!);
                PaneItems.Insert(index, pane!);
                PaneItems.RemoveAt(index + 1);
                if (lastPane?.Equals(pane) ?? false)
                    SelectedPane = pane;
            });
        }

        private void SettingsService_ConfigReloaded(object? sender, EventArgs e)
        {
            _viewService.DispatcherEnqueue(() =>
            {
                var lastPane = SelectedPane;

                SelectedPane = null;
                PaneItems.Clear();
                PopulatePanes();

                // return to last pane if it's still there
                lastPane = PaneItems.FirstOrDefault(item => item.Equals(lastPane));
                if (lastPane != null)
                    SelectedPane = lastPane;
            });
        }

        private void OnClose(object? sender, WndProcEventArgs args)
        {
            // save WindowPlacement when closing window
            if (_view is Window window)
            {
                if (window.Handle == IntPtr.Zero) return;
                var placement = window.GetWindowPlacement();
                var serializer = new BinaryFormatter();
                using var stream = new MemoryStream();
                serializer.Serialize(stream, placement);
                var bytes = stream.ToArray();
                _settingsContainer.SetValue("WindowPlacement", bytes);
            }
        }

        // commands 

        private void DoClose()
        {
            _view?.Close();
        }

        private void DoAddProcessRule()
        {
            AddProcessRuleDialog dialog = new();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += async (sender, args) =>
            {
                _newRule = new ProcessRule(dialog.ViewModel.ProcessName);
                await _settingsService.AddRuleAsync(_newRule);
            };

            _dialogService.ShowDialog(_view as Window, dialog);
        }

        private void DoAddClassRule()
        {
            AddClassRuleDialog dialog = new();
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.ViewModel.Submit += async (sender, args) =>
            {
                _newRule = new ClassRule(dialog.ViewModel.ClassName);
                await _settingsService.AddRuleAsync(_newRule);
            };

            _dialogService.ShowDialog(_view as Window, dialog);
        }

        private async Task DoRemoveRuleAsync()
        {
            if (SelectedPane is RulePaneItem { ViewModel: { Rule: { } } viewModel })
            {
                SelectedPane = _generalPane;
                await _settingsService.RemoveRuleAsync(viewModel.Rule);
            }
        }

        private bool CanRemoveRule() => SelectedPane is { ItemType: not (PaneItemType.General or PaneItemType.Global) };
    }
}
