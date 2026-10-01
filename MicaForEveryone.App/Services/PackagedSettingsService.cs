using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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

    private FileSystemWatcher? watcher;

    private bool recentWriteDueToApp = false;

    private Task pendingSave = Task.CompletedTask;

    private IDispatchingService _dispatching;

    private SemaphoreSlim semaphore = new(1, 1);

    public PackagedSettingsService(IDispatchingService dispatching)
    {
        _dispatching = dispatching;
    }

    public async Task InitializeAsync()
    {
        var folder = Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalPath;
        Stream stream;
        try
        {
            stream = new FileStream($"{folder}\\{SettingsFileName}", FileMode.Open, FileAccess.Read);
        }
        catch
        {
            File.Copy($"{Package.Current.InstalledPath}\\Assets\\default.json", $"{folder}\\{SettingsFileName}", true);
            stream = new FileStream($"{Package.Current.InstalledPath}\\Assets\\default.json", FileMode.Open, FileAccess.Read);
        }
        Settings = await JsonSerializer.DeserializeAsync(stream, MFESerializerContext.Default.SettingsFileModel);
        stream.Close();
        watcher = new FileSystemWatcher(folder);
        watcher.Changed += Watcher_Changed;
        watcher.EnableRaisingEvents = true;
    }

    public async Task OpenConfigurationFileAsync()
    {
        StorageFile file = await Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalFolder.GetFileAsync(SettingsFileName);
        await Windows.System.Launcher.LaunchFileAsync(file);
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs args)
    {
        _ = WatcherChangedAsync(args.Name!);
    }

    private async Task WatcherChangedAsync(string name)
    {
        if (name != string.Empty && !name.Equals(SettingsFileName, StringComparison.CurrentCultureIgnoreCase))
            return;

        if (semaphore.CurrentCount == 0)
            return;

        await semaphore.WaitAsync();

        await Task.Delay(200);

        if (recentWriteDueToApp == true)
        {
            recentWriteDueToApp = false;
            goto RELEASE_SEMAPHORE;
        }

        await _dispatching.YieldAsync();

        // TODO: Finish this
        using (Stream settingsStream = new FileStream($"{Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalPath}\\{SettingsFileName}", FileMode.Open, FileAccess.Read))
        {
            Settings = await JsonSerializer.DeserializeAsync(settingsStream, MFESerializerContext.Default.SettingsFileModel);
        }
        Debug.WriteLine("Reloaded");

RELEASE_SEMAPHORE:
        semaphore.Release();
    }

    public Task SaveAsync() => pendingSave = SaveCoreAsync();

    public async Task WaitForPendingSaveAsync()
    {
        Task save;
        do
        {
            save = pendingSave;
            await save;
        }
        while (!ReferenceEquals(save, pendingSave));
    }

    private async Task SaveCoreAsync()
    {
        using (Stream settingsStream = new FileStream($"{Microsoft.Windows.Storage.ApplicationData.GetDefault().LocalPath}\\{SettingsFileName}", FileMode.Open, FileAccess.Write))
        {
            recentWriteDueToApp = true;
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
partial class MFESerializerContext : JsonSerializerContext;
