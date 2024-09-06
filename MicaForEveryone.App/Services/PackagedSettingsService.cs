using MicaForEveryone.App.Helpers;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace MicaForEveryone.App.Services;

public sealed partial class PackagedSettingsService : ISettingsService
{
    private SettingsFileModel? _settings = new() { Rules = new() };

    public SettingsFileModel? Settings
    {
        get => _settings;
        set
        {
            if (_settings != value)
            {
                _settings = value;
                PropertyChanged?.Invoke(this, new(nameof(Settings)));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private const string SettingsFileName = "settings.json";

    private StorageFile? settingsFile;

    private WinRTFileSystemWatcher? watcher;

    private int recentWriteDueToApp = 0;

    private IDispatchingService _dispatching;

    public PackagedSettingsService(IDispatchingService dispatching)
    {
        _dispatching = dispatching;
    }

    public async Task InitializeAsync()
    {
        var folder = ApplicationData.Current.LocalFolder;
        var file = await folder.TryGetItemAsync(SettingsFileName);
        if (file is null)
        {
            StorageFile defaultFile = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///Assets/default.json"));
            await defaultFile.CopyAsync(ApplicationData.Current.LocalFolder, SettingsFileName);
        }
        settingsFile = await folder.GetFileAsync(SettingsFileName);
        using Stream settingsStream = await settingsFile.OpenStreamForReadAsync();
        Settings = await JsonSerializer.DeserializeAsync(settingsStream, MFESerializerContext.Default.SettingsFileModel);
        watcher = new(folder, WinRTFileSystemWatcher.NotifyFilters.FileName | WinRTFileSystemWatcher.NotifyFilters.LastWrite | WinRTFileSystemWatcher.NotifyFilters.Size, false);
        watcher.Changed += Watcher_Changed;
    }

    private void Watcher_Changed(WatcherChangeTypes changeTypes, ReadOnlySpan<char> name) => _ = WatcherChangedAsync(changeTypes, name.ToString());

    private async Task WatcherChangedAsync(WatcherChangeTypes changeTypes, string name)
    {
        if (!name.Equals(SettingsFileName, StringComparison.CurrentCultureIgnoreCase))
            return;

        if (recentWriteDueToApp != 0)
        {
            recentWriteDueToApp++;
            if (recentWriteDueToApp == 3)
                recentWriteDueToApp = 0;
            return;
        }

        await _dispatching.YieldAsync();

        using Stream settingsStream = await settingsFile!.OpenStreamForReadAsync();
        Settings = await JsonSerializer.DeserializeAsync(settingsStream, MFESerializerContext.Default.SettingsFileModel);
    }

    public async Task SaveAsync()
    {
        using (Stream settingsStream = (await settingsFile!.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.AllowOnlyReaders)).AsStreamForWrite())
        {
            recentWriteDueToApp = 1;
            settingsStream.SetLength(0);
            await JsonSerializer.SerializeAsync(settingsStream!, Settings!, MFESerializerContext.Default.SettingsFileModel);
        }
    }

    public void Dispose()
    {
        watcher?.Dispose();
    }
}

[JsonSerializable(typeof(SettingsFileModel))]
[JsonSourceGenerationOptions(WriteIndented = false, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, UseStringEnumConverter = true)]
partial class MFESerializerContext : JsonSerializerContext
{

}