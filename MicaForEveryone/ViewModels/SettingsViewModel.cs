using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : BaseViewModel, ISettingsViewModel
    {
        private readonly IConfigService _configService;

        public SettingsViewModel(IConfigService configService)
        {
            _configService = configService;
            CloseCommand = new RelyCommand(Close);
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

        public ICommand CloseCommand { get; }

        private void Close(object obj)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.SettingsWindow?.Close();
        }
    }
}
