using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Core;
using Windows.UI.Xaml;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

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
        }

        ~SettingsViewModel()
        {
            _configService.Updated -= ConfigService_Changed;
        }

        public bool SystemBackdropIsSupported { get; } =
#if !DEBUG
            SystemBackdrop.IsSupported;
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
                SelectedPane = null;
                PaneItems.Clear();
                PopulatePanes();
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
            throw new NotImplementedException();
        }

        private void AddClassRule(object obj)
        {
            throw new NotImplementedException();
        }

        private void RemoveRule(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanRemoveRule(object obj) => SelectedPane != null &&
            SelectedPane.ItemType is not (PaneItemType.General or PaneItemType.Global);
    }
}
