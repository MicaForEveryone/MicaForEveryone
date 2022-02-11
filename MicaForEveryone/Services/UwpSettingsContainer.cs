using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

using MicaForEveryone.Interfaces;

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

        public object GetValue(string key)
        {
            return _container.Values[key];
        }

        public void SetValue(string key, object value)
        {
            _container.Values[key] = value;
        }
    }
}
