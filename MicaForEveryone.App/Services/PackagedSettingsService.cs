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
    private FileSystemWatcher? _watcher;

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

    public async Task InitializeAsync()
    {
        Stream contentStream;
        var folder = ApplicationData.Current.LocalFolder;
        var file = await folder.TryGetItemAsync("settings.json");
        if (file == null)
        {
            StorageFile defaultFile = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///Assets/DefaultConfiguration.json"));
            contentStream = await defaultFile.OpenStreamForReadAsync();
            await defaultFile.CopyAsync(ApplicationData.Current.LocalFolder, "settings.json");
        }
        else
        {
            contentStream = await ((StorageFile)file).OpenStreamForReadAsync();
        }

        Settings = await JsonSerializer.DeserializeAsync(contentStream, MFESerializationContext.Default.SettingsModel);

        _watcher = new(folder.Path, "settings.json");
        _watcher.Changed += (_, e) =>
        {
            _ = HandleChangeAsync(e);

            async Task HandleChangeAsync(FileSystemEventArgs args)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(args.FullPath);
                using var stream = await file.OpenStreamForReadAsync();
                Settings = await JsonSerializer.DeserializeAsync(stream, MFESerializationContext.Default.SettingsModel);
            }
        };
        _watcher.EnableRaisingEvents = true;
    }

    public async Task SaveAsync()
    {
        using (_ = await _lock.LockAsync())
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.json", CreationCollisionOption.ReplaceExisting);
            using var stream = await file.OpenStreamForWriteAsync();
            await JsonSerializer.SerializeAsync(stream, Settings!, MFESerializationContext.Default.SettingsModel);
        }
    }
}
