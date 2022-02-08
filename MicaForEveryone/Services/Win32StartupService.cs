using System;
using System.Diagnostics;
using Microsoft.Win32;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.Services
{
    internal class Win32StartupService : IStartupService
    {
        private const string StartupRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string MicaForEveryoneStartupName = "Mica For Everyone";

        private readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName;

        public bool GetEnabled()
        {
            var autorunPath = Registry.CurrentUser.OpenSubKey(StartupRegistryKey).GetValue(MicaForEveryoneStartupName) as string;
#if DEBUG
            Debug.WriteLine("Autorun Path Get: " + autorunPath);
#endif
            return autorunPath == ExecutablePath;
        }

        public void SetEnabled(bool state)
        {
            if (state == true)
            {
                Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true).SetValue(MicaForEveryoneStartupName, ExecutablePath);
#if DEBUG
                Debug.WriteLine("Autorun Path Set: " + ExecutablePath);
#endif
                return;
            }
            Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true).DeleteValue(MicaForEveryoneStartupName);
        }
    }
}
