using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Models;
using System.Collections.ObjectModel;
using System;

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : BaseViewModel, ISettingsViewModel
    {
        private readonly IConfigService _configService;
        private readonly IViewService _viewService;

        public SettingsViewModel(IConfigService configService, IViewService viewService)
        {
            _configService = configService;
            _viewService = viewService;
            CloseCommand = new RelyCommand(Close);

            _viewService.MainWindow.ViewModel.PropertyChanged += TrayIconViewModel_PropertyChanged;

            BackdropTypesSource.Add(BackdropType.Default);
            BackdropTypesSource.Add(BackdropType.None);
            BackdropTypesSource.Add(BackdropType.Mica);

            #if !DEBUG
            if (SystemBackdropIsSupported)
            #endif
            {
                BackdropTypesSource.Add(BackdropType.Acrylic);
                BackdropTypesSource.Add(BackdropType.Tabbed);
            }

            foreach (TitlebarColorMode item in Enum.GetValues(typeof(TitlebarColorMode)))
            {
                TitlebarColorModesSource.Add(item);
            }
        }

        ~SettingsViewModel()
        {
            _viewService.MainWindow.ViewModel.PropertyChanged -= TrayIconViewModel_PropertyChanged;
        }

        public bool ReloadOnChange
        {
            get
            {
                return _configService.ConfigSource.GetWatchState();
            }
            set
            {
                _configService.ConfigSource.SetWatchState(value);
                OnPropertyChanged();
            }
        }

        public bool SystemBackdropIsSupported
        {
            get => SystemBackdrop.IsSupported;
        }

        public BackdropType BackdropType
        {
            get => _viewService.MainWindow.ViewModel.BackdropType;
            set => _viewService.MainWindow.ViewModel.BackdropType = value;
        }

        public TitlebarColorMode TitlebarColor
        {
            get => _viewService.MainWindow.ViewModel.TitlebarColor;
            set => _viewService.MainWindow.ViewModel.TitlebarColor = value;
        }

        public bool ExtendFrameIntoClientArea
        {
            get => _viewService.MainWindow.ViewModel.ExtendFrameIntoClientArea;
            set => _viewService.MainWindow.ViewModel.ExtendFrameIntoClientArea = value;
        }

        public Version Version { get; } = typeof(Program).Assembly.GetName().Version;

        public ICommand CloseCommand { get; }

        public ObservableCollection<BackdropType> BackdropTypesSource { get; } = new ObservableCollection<BackdropType>();

        public ObservableCollection<TitlebarColorMode> TitlebarColorModesSource { get; } = new ObservableCollection<TitlebarColorMode>();

        private void Close(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.SettingsWindow?.Close();
        }

        private void TrayIconViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (new[]
            {
                nameof(BackdropType),
                nameof(TitlebarColor),
                nameof(ExtendFrameIntoClientArea),
            }.Contains(e.PropertyName))
            {
                OnPropertyChanged(e.PropertyName);
            }
        }
    }
}
