using System;
using Vanara.PInvoke;

namespace MicaForEveryone
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                User32.MessageBox(HWND.NULL, "This app require at least Windows 11 (10.0.22000.0) to work.", "Error",
                    User32.MB_FLAGS.MB_OK | User32.MB_FLAGS.MB_ICONERROR);
                Environment.Exit(1);
                return;
            }
            
            new MicaForEveryoneApp().Run();
        }
    }
}
