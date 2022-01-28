using Windows.UI.Xaml;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Views;

namespace MicaForEveryone.Services
{
    internal class ViewService : IViewService
    {
        public MainWindow MainWindow { get; private set; }

        public void Run(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            mainWindow.Activate();
            Program.CurrentApp.Run(mainWindow);
        }

        public TitlebarColorMode SystemColorMode => Application.Current.RequestedTheme switch
        {
            ApplicationTheme.Light => TitlebarColorMode.Light,
            ApplicationTheme.Dark => TitlebarColorMode.Dark,
            _ => TitlebarColorMode.Default,
        };
    }
}
