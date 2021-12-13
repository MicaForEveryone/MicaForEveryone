using System;
using System.Windows.Forms;

namespace MicaForEveryone
{
    internal static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            if (Environment.OSVersion.Version.Build < 22000) {
                MessageBox.Show("This app requires Windows 11.", "Error", MessageBoxButtons.OK);
                Environment.Exit(1);
                return;
            }

            Application.Run(new MicaForEveryoneApp());
        }
    }
}
