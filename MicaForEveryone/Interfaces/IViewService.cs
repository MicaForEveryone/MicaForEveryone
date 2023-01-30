using System;

using MicaForEveryone.Views;

namespace MicaForEveryone.Interfaces
{
    internal interface IViewService : IDisposable
    {
        MainWindow MainWindow { get; }
        SettingsWindow SettingsWindow { get; }

        void Run();
        void ShowSettingsWindow();
    }
}
