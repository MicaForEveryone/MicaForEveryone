using System;

namespace MicaForEveryone
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Build < 18362)
            {
                Environment.Exit(1);
                return;
            }

            using var app = new App();
            app.Run();
        }
    }
}
