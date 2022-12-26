using System;

namespace MicaForEveryone
{
    internal static class Program
    {
        public static App CurrentApp { get; } = new();

        [STAThread]
        public static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Build < 18362)
            {
                Environment.Exit(1);
                return;
            }

            if (args.Length > 0 && args[0] == "--uninstall")
            {
                Services.UninstallService.Run();
                return;
            }
            
            if (!CurrentApp.IsItFirstInstance())
            {
                var msg = Win32.Window.RegisterWindowMessage(Views.MainWindow.OpenSettingsMessage);
                Win32.Window.Broadcast(msg);
                return;
            }

            CurrentApp.Run();
            CurrentApp.Dispose();
        }
    }
}
