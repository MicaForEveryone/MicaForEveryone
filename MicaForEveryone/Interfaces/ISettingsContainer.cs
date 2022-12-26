using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    public interface ISettingsContainer : IDisposable
    {
        void SetValue(string key, object? value);
        object? GetValue(string key);
        void Flush();
    }
}
