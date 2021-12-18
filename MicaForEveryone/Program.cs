using System;
using Vanara.PInvoke;

using MicaForEveryone.Rules;
using MicaForEveryone.UWP;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;

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

            using var app = new App();
            app.Run();
        }
    }
}
