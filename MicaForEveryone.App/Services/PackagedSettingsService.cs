using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace MicaForEveryone.App.Services;

public sealed class PackagedSettingsService : ISettingsService
{
    private SettingsModel? _settings;
    private FileSystemWatcher? _watcher;

    public event PropertyChangedEventHandler? PropertyChanged;

    public SettingsModel? Settings
    {
        get => _settings;
        set
        {
            if (_settings == value)
                return;

            _settings = value;

            PropertyChanged?.Invoke(this, new(nameof(Settings)));
        }
    }

    public async Task InitializeAsync()
    {
        Stream contentStream;
        var folder = ApplicationData.Current.LocalFolder;
        var file = await folder.TryGetItemAsync("settings.json");
        if (file is null)
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
        var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("settings.json", CreationCollisionOption.ReplaceExisting);
        using var stream = await file.OpenStreamForWriteAsync();
        await JsonSerializer.SerializeAsync(stream, Settings!, MFESerializationContext.Default.SettingsModel);
    }
}
