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

            CurrentApp.Run();
            CurrentApp.Dispose();
        }
    }
}
