using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using NeoSmart.AsyncLock;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace MicaForEveryone.App.Services;

public sealed class PackagedSettingsService : ISettingsService
{
    private SettingsModel? _settings;
    private readonly AsyncLock _lock = new();

    public SettingsModel? Settings
    {
        get => _settings;
        set
        {
            if (_settings == value)
                return;

            using (_ = _lock.Lock())
            {
                bool wasNull = _settings is null;
                _settings = value;

                if (!wasNull)
                    _ = SaveAsync();
            }
        }
    }

    private static string? _cachedPath;

    public static string GetSettingsPath()
    {
        return _cachedPath ??= Path.Combine(ApplicationData.Current.LocalFolder.Path, "settings.json");
    }

    public async Task InitializeAsync()
    {
        string contents;
        var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("settings.json");
        if (file == null)
        {
            StorageFile defaultFile = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///Assets/DefaultConfiguration.json"));
            contents = await FileIO.ReadTextAsync(defaultFile);
            await defaultFile.CopyAsync(ApplicationData.Current.LocalFolder, "settings.json");
        }
        else
        {
            contents = await FileIO.ReadTextAsync((StorageFile)file);
        }

        Settings = JsonSerializer.Deserialize(contents, MFESerializationContext.Default.SettingsModel);
    }

    public async Task SaveAsync()
    {
        using (_ = await _lock.LockAsync())
        {
            await FileIO.WriteTextAsync(await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.json", CreationCollisionOption.ReplaceExisting), JsonSerializer.Serialize(Settings!, MFESerializationContext.Default.SettingsModel));
        }
    }
}
