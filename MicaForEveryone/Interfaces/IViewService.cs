using System;

using MicaForEveryone.Views;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    internal interface IViewService : IDisposable
    {
        MainWindow? MainWindow { get; }
        SettingsWindow? SettingsWindow { get; }

        void Initialize(Application app);
        void Unload();
        void ShowSettingsWindow();
        void DispatcherEnqueue(Action action);
    }
}
