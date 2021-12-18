using System;

using MicaForEveryone.Rules;
using MicaForEveryone.Win32;

namespace MicaForEveryone
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                MessageBox.ShowErrorMessage("This app require at least Windows 11 (10.0.22000.0) to work.");
                Environment.Exit(1);
                return;
            }

            using var uwpApp = new MicaForEveryone.UWP.App();

            using var app = new Application();

            app.RuleHandler.ConfigSource = new ConfigFileReader(
                args.Length > 1 ? args[2] : "config.ini"
            );
            app.RuleHandler.LoadConfig();

            app.Run();
        }
    }
}
