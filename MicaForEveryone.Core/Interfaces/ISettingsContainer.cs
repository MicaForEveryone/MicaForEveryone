#nullable enable

namespace MicaForEveryone.Core.Interfaces
{
    public interface ISettingsContainer : IDisposable
    {
        void SetValue(string key, object? value);
        object? GetValue(string key);
        void Flush();
    }
}
