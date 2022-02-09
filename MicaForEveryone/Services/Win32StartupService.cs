using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.Services
{
    internal class Win32StartupService : IStartupService, IDisposable
    {
        private const string StartupRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string MicaForEveryoneStartupName = "Mica For Everyone";

        private readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName;

        private RegistryKey _key;

        public void Initialize()
        {
            _key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true);
        }

        public bool IsAvailable => true;

        public bool IsEnabled
        {
            get
            {
                var autorunPath = _key.GetValue(MicaForEveryoneStartupName) as string;
                return autorunPath == ExecutablePath;
            }
        }

        public Task<bool> SetStateAsync(bool state)
        {
            return Task.Run(() =>
            {
                if (state == true)
                {
                    _key.SetValue(MicaForEveryoneStartupName, ExecutablePath);
                    _key.Flush();
                    return true;
                }

                _key.DeleteValue(MicaForEveryoneStartupName);
                _key.Flush();
                return false;
            });
        }

        public void Dispose()
        {
            _key.Dispose();
        }
    }
}
