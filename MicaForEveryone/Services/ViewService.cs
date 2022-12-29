using System;
using Windows.UI.Xaml;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Views;

namespace MicaForEveryone.Services
{
    internal class ViewService : IViewService
    {
        public MainWindow MainWindow { get; private set; }
        public SettingsWindow SettingsWindow { get; private set; }
        public LogsWindow LogsWindow { get; private set; }

        public void Run()
        {
            if (MainWindow != null) return;
            MainWindow = new MainWindow();
            MainWindow.Destroy += MainWindow_Destroy;
            MainWindow.Activate();
            Program.CurrentApp.Run(MainWindow);
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

        public void ShowLogsWindow()
        {
			if (LogsWindow == null)
			{
				LogsWindow = new LogsWindow();
				LogsWindow.Destroy += LogsWindow_Destroy;

				LogsWindow.Activate();
			}
			else
			{
				LogsWindow.SetForegroundWindow();
			}
		}

        private void MainWindow_Destroy(object sender, Win32.WndProcEventArgs e)
        {
            MainWindow.Dispose();
            MainWindow = null;
        }

        private void SettingsWindow_Destroy(object sender, Win32.WndProcEventArgs e)
        {
            SettingsWindow.Dispose();
            SettingsWindow = null;
        }

		private void LogsWindow_Destroy(object sender, Win32.WndProcEventArgs e)
		{
			LogsWindow.Dispose();
			LogsWindow = null;
		}

		public void Dispose()
        {
            MainWindow?.Dispose();
            SettingsWindow?.Dispose();
        }
    }
}
