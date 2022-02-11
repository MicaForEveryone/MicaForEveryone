using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;

using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class Win32StartupService : IStartupService
    {
        private const string StartupRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string MicaForEveryoneStartupName = "Mica For Everyone";

        private readonly string ExecutablePath = Process.GetCurrentProcess().MainModule.FileName;

        private RegistryKey? _key;

        public Task InitializeAsync()
        {
            return Task.Run(() =>
            {
                _key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true) ??
                    Registry.CurrentUser.CreateSubKey(StartupRegistryKey, true);
            });
        }

        public bool IsAvailable => true;

        public bool IsEnabled
        {
            get
            {
                var autorunPath = _key!.GetValue(MicaForEveryoneStartupName) as string;
                return autorunPath == ExecutablePath;
            }
        }

        public Task<bool> SetStateAsync(bool state)
        {
            return Task.Run(() =>
            {
                if (state == true)
                {
                    _key!.SetValue(MicaForEveryoneStartupName, ExecutablePath);
                    _key.Flush();
                    return true;
                }

                _key!.DeleteValue(MicaForEveryoneStartupName);
                _key.Flush();
                return false;
            });
        }

        public void Dispose()
        {
            _key?.Dispose();
        }
    }
}
