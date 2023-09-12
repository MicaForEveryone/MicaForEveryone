using MicaForEveryone.CoreUI;
using System.IO;
using Windows.Storage;

namespace MicaForEveryone.App.Services;

public sealed class PackagedSettingsService : ISettingsService
{
    public string GetPathForFileName(string fileName)
    {
        return Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
    }
}
