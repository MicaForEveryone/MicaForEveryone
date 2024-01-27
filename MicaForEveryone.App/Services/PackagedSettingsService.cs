using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using XclParser;

namespace MicaForEveryone.App.Services;

public sealed class PackagedSettingsService : ISettingsService
{
    private readonly IParserService _parserService;
    private FileSystemWatcher? _watcher;

    public event PropertyChangedEventHandler? PropertyChanged;

    public PackagedSettingsService(IParserService parserService)
    {
        _parserService = parserService;
    }

    public async Task InitializeAsync()
    {
        Stream contentStream;
        var folder = ApplicationData.Current.LocalFolder;
        var file = await folder.TryGetItemAsync("rules.config");
        if (file is null)
        {
            /*
            StorageFile defaultFile = await StorageFile.GetFileFromApplicationUriAsync(new("ms-appx:///Assets/defaultrules.config"));
            contentStream = await defaultFile.OpenStreamForReadAsync();
            await defaultFile.CopyAsync(ApplicationData.Current.LocalFolder, "rules.config");
            */
        }
        else
        {
            contentStream = await ((StorageFile)file).OpenStreamForReadAsync();
        }

        _watcher = new(folder.Path, "rules.config");
        _watcher.Changed += (_, e) =>
        {
            _ = HandleChangeAsync(e);

            async Task HandleChangeAsync(FileSystemEventArgs args)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(args.FullPath);
                using var stream = await file.OpenStreamForReadAsync();
                
            }
        };
        _watcher.EnableRaisingEvents = true;
    }

    public async Task SaveAsync()
    {
    }
}
