using System;
using MicaForEveryone.Win32;

namespace MicaForEveryone
{
    internal static class Program
    {
        public static App CurrentApp { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                Environment.Exit(1);
                return;
            }

            var context = Application.AddDynamicDependency("Microsoft.UI.Xaml");
            CurrentApp = new App();

            if (!CurrentApp.IsItFirstInstance())
            {
                var msg = Win32.Window.RegisterWindowMessage(Views.MainWindow.OpenSettingsMessage);
                Win32.Window.Broadcast(msg);
                return;
            }

            CurrentApp.Run();
            CurrentApp.Dispose();

            Application.RemoveDynamicDependency(context);
        }
    }
}
