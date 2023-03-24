using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

using MicaForEveryone.Core.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class UwpConfigFile : IConfigFile
    {
        private readonly FileSystemWatcher _fileSystemWatcher = new();

        private StorageFile? _file;
        private bool _isFileWatcherEnabled;

        public UwpConfigFile(IConfigParser parser)
        {
            Parser = parser;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
        }

        public IConfigParser Parser { get; }

        public string? FilePath { get; set; }

        public bool IsFileWatcherEnabled
        {
            get => _isFileWatcherEnabled;
            set
            {
                _isFileWatcherEnabled = value;
                if (_fileSystemWatcher.Path is not (null or ""))
                {
                    _fileSystemWatcher.EnableRaisingEvents = value;
                }
            }
        }

        public async Task InitializeAsync()
        {
            if (_file != null && _file.Path == FilePath)
                return;

            var parent = ApplicationData.Current.LocalFolder;

            if (FilePath == null)
            {
                if (await ApplicationData.Current.LocalFolder.TryGetItemAsync("MicaForEveryone.conf") == null)
                {
                    var bundled = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///MicaForEveryone/MicaForEveryone.conf"));
                    await bundled.CopyAsync(ApplicationData.Current.LocalFolder);
                }
                _file = await ApplicationData.Current.LocalFolder.GetFileAsync("MicaForEveryone.conf");
                FilePath = _file.Path;
            }
            else
            {
                _file = await StorageFile.GetFileFromPathAsync(FilePath);
                parent = await _file.GetParentAsync();
            }

            _fileSystemWatcher.Path = parent.Path;
            _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
        }

        public async Task ResetAsync()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            try
            {
                var bundled = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///MicaForEveryone/MicaForEveryone.conf"));
                await bundled.CopyAndReplaceAsync(_file);
            }
            finally
            {
                _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
            }
        }

        public async Task<IRule[]> LoadAsync()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            try
            {
                using var stream = await _file.OpenStreamForReadAsync();
                using var streamReader = new StreamReader(stream);
                await Parser.LoadAsync(streamReader);
                return Parser.Rules;
            }
            finally
            {
                _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
            }
        }

        public async Task SaveAsync()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            try
            {
                using var stream = await _file.OpenStreamForWriteAsync();
                stream.SetLength(0);
                using var streamWriter = new StreamWriter(stream);
                await Parser.SaveAsync(streamWriter);
            }
            finally
            {
                _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
            }
        }

        public void Dispose()
        {
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs args)
        {
            if (IsFileWatcherEnabled && args.Name == _file?.Name)
                FileChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? FileChanged;
    }
}
