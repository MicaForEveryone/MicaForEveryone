using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.ViewModels
{
    internal class SettingsViewModel : BaseViewModel, ISettingsViewModel
    {
        public SettingsViewModel()
        {
            CloseCommand = new RelyCommand(Close);
        }

        public bool ReloadOnChange
        {
            get
            {
                var configService = Program.CurrentApp.Container.GetService<IConfigService>();
                return configService.ConfigSource.GetWatchState();
            }
            set
            {
                var configService = Program.CurrentApp.Container.GetService<IConfigService>();
                configService.ConfigSource.SetWatchState(value);
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
