using Windows.Storage;

using MicaForEveryone.Core.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class UwpSettingsContainer : ISettingsContainer
    {
        private readonly ApplicationDataContainer _container;

        public UwpSettingsContainer()
        {
            _container = ApplicationData.Current.LocalSettings;
        }

        public void Dispose()
        {
        }

        public void Flush()
        {
        }

        public object? GetValue(string key)
        {
            if (_container.Values.ContainsKey(key) == false) return null;
            return _container.Values[key];
        }

        public void SetValue(string key, object? value)
        {
            _container.Values[key] = value;
        }
    }
}
