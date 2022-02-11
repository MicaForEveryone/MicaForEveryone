using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class UwpConfigFile : IConfigFile
    {
        private StorageFile? _file;
        private StorageFileQueryResult? _query;

        public UwpConfigFile(IConfigParser parser)
        {
            Parser = parser;
        }

        public IConfigParser Parser { get; }

        public string? FilePath { get; set; }

        public bool IsFileWatcherEnabled { get; set; }

        public async Task InitializeAsync()
        {
            if (_file != null && _file.Path == FilePath)
                return;

            if (_query != null)
                _query.ContentsChanged -= Query_ContentsChanged;

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

            _query = parent.CreateFileQuery();
            _query.ContentsChanged += Query_ContentsChanged;
            await _query.GetFilesAsync(); // needs to be called to ContentsChanged get fired
        }

        public async Task ResetAsync()
        {
            var bundled = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///MicaForEveryone/MicaForEveryone.conf"));
            await bundled.CopyAndReplaceAsync(_file);
        }

        public async Task<IRule[]> LoadAsync()
        {
            using var stream = await _file.OpenStreamForReadAsync();
            using var streamReader = new StreamReader(stream);
            await Parser.LoadAsync(streamReader);
            return Parser.Rules;
        }

        public async Task SaveAsync()
        {
            using var stream = await _file.OpenStreamForWriteAsync();
            using var streamWriter = new StreamWriter(stream);
            await Parser.SaveAsync(streamWriter);
        }

        public void Dispose()
        {
        }

        private void Query_ContentsChanged(IStorageQueryResultBase sender, object args)
        {
            if (IsFileWatcherEnabled)
            {
                try
                {
                    IsFileWatcherEnabled = false;
                    FileChanged?.Invoke(this, EventArgs.Empty);
                }
                finally
                {
                    IsFileWatcherEnabled = true;
                }
            }
        }

        public event EventHandler? FileChanged;
    }
}
