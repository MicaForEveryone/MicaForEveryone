using System;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class ViewService : IViewService
    {
        private readonly IUiSettingsService _uiSettingsService;

        private Application? _app;

        public ViewService(IUiSettingsService uiSettingsService)
        {
            _uiSettingsService = uiSettingsService;
            _uiSettingsService.LanguageChanged += UiSettingsService_OnLanguageChanged;
        }

        ~ViewService()
        {
            _uiSettingsService.LanguageChanged -= UiSettingsService_OnLanguageChanged;
        }

        public MainWindow? MainWindow { get; private set; }
        public SettingsWindow? SettingsWindow { get; private set; }

        public void Initialize(Application app)
        {
            _app = app;
            if (MainWindow != null) return;
            MainWindow = new MainWindow();
            MainWindow.Destroy += MainWindow_Destroy;
            MainWindow.Activate();
            _app.AddWindow(MainWindow);
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

        public void Dispose()
        {
            MainWindow?.Dispose();
            SettingsWindow?.Dispose();
        }

        private void MainWindow_Destroy(object? sender, Win32.WndProcEventArgs e)
        {
            MainWindow?.Dispose();
            MainWindow = null;
            _app?.Exit();
        }

        private void SettingsWindow_Destroy(object? sender, Win32.WndProcEventArgs e)
        {
            SettingsWindow?.Dispose();
            SettingsWindow = null;
        }

        private void UiSettingsService_OnLanguageChanged(object? sender, EventArgs e)
        {
            if (_app == null) return;
            
            var showSettingsWindow = SettingsWindow != null;
            if (showSettingsWindow)
            {
                SettingsWindow?.Close();
                SettingsWindow?.Dispose();
                SettingsWindow = null;
            }

            MainWindow!.Destroy -= MainWindow_Destroy;
            MainWindow.Close();
            MainWindow.Dispose();
            MainWindow = null;
            
            Initialize(_app);
            
            if (showSettingsWindow)
            {
                ShowSettingsWindow();
            }
        }
    }
}
