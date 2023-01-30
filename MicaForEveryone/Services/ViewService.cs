using System;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class ViewService : IViewService
    {
        private Application? _app;

        public MainWindow? MainWindow { get; private set; }
        public SettingsWindow? SettingsWindow { get; private set; }
        
        public void Initialize(Application app)
        {
            _app = app;
            if (MainWindow != null) return;
            MainWindow = new MainWindow();
            MainWindow.Destroy += MainWindow_Destroy;
            MainWindow.Activate();
        }

        public void Unload()
        {
            MainWindow?.Close();
            MainWindow?.Dispose();
            MainWindow = null;
            _app = null;
        }

        public void ShowSettingsWindow()
        {
            if (SettingsWindow == null)
            {
                SettingsWindow = new SettingsWindow();
                SettingsWindow.Destroy += SettingsWindow_Destroy;

                SettingsWindow.Activate();
            }
            else
            {
                SettingsWindow.SetForegroundWindow();
            }
        }

        public void DispatcherEnqueue(Action action)
        {
            _app!.Dispatcher.Enqueue(action);
        }

        private void MainWindow_Destroy(object? sender, Win32.WndProcEventArgs e)
        {
            MainWindow?.Dispose();
            MainWindow = null;
        }

        private void SettingsWindow_Destroy(object? sender, Win32.WndProcEventArgs e)
        {
            SettingsWindow?.Dispose();
            SettingsWindow = null;
        }

        public void Dispose()
        {
            MainWindow?.Dispose();
            SettingsWindow?.Dispose();
        }
    }
}
