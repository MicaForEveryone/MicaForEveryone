using MicaForEveryone.Models;
using MicaForEveryone.Views;

namespace MicaForEveryone.Interfaces
{
    internal interface IViewService
    {
        TitlebarColorMode SystemColorMode { get; }

        MainWindow MainWindow { get; }
        SettingsWindow SettingsWindow { get; }

        void Run();
        void ShowSettingsWindow();
    }
}
