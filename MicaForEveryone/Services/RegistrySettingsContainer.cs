using Microsoft.Win32;

using MicaForEveryone.Core.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class RegistrySettingsContainer : ISettingsContainer
    {
        private const string SettingsRegistryKey = @"Software\MicaForEveryone";

        private readonly RegistryKey _key;

        public RegistrySettingsContainer()
        {
            _key = Registry.CurrentUser.OpenSubKey(SettingsRegistryKey, true) ??
                Registry.CurrentUser.CreateSubKey(SettingsRegistryKey, true);
        }

        public void Dispose()
        {
            _key.Dispose();
        }

        public void Flush()
        {
            _key.Flush();
        }

        public object? GetValue(string key)
        {
            return _key.GetValue(key);
        }

        public void SetValue(string key, object? value)
        {
            if (value == null)
            {
                _key.DeleteValue(key, false);
                return;
            }
            _key.SetValue(key, value);
        }
    }
}
