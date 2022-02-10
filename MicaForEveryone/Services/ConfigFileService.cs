using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using MicaForEveryone.Interfaces;

namespace MicaForEveryone.Services
{
    internal class ConfigFileService : IConfigFile, IDisposable
    {
        private static async Task<FileStream> TryOpenFile(string filePath, FileMode mode, FileAccess access, int timeout = 1000)
        {
            return await Task.Run(() =>
            {
                var time = Stopwatch.StartNew();
                while (time.ElapsedMilliseconds < timeout)
                {
                    try
                    {
                        return File.Open(filePath, mode, access);
                    }
                    catch (IOException exception)
                    {
                        if (exception.HResult != -2147024864)
                            throw;
                    }
                }
                throw new TimeoutException($"Failed to get a write handle to {filePath} within {timeout}ms.");
            });
        }

        private readonly FileSystemWatcher _fileSystemWatcher = new();

        private string _filePath;
        private string _fileName;

        public ConfigFileService(IConfigParser parser)
        {
            Parser = parser;
        }

        public IConfigParser Parser { get; }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                _fileName = Path.GetFileName(value);
                var directoryPath = Directory.GetParent(value).FullName;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                _fileSystemWatcher.Path = directoryPath;
                _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            }
        }

        public bool IsFileWatcherEnabled
        {
            get => _fileSystemWatcher.EnableRaisingEvents;
            set => _fileSystemWatcher.EnableRaisingEvents = value;
        }

        public async Task<IRule[]> LoadAsync()
        {
            using var stream = await TryOpenFile(FilePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            await Parser.LoadAsync(reader);
            return Parser.Rules;
        }

        public async Task SaveAsync()
        {
            var lastWatcherState = IsFileWatcherEnabled;
            try
            {
                IsFileWatcherEnabled = false;
                using var stream = await TryOpenFile(FilePath, FileMode.Create, FileAccess.Write);
                using var writer = new StreamWriter(stream);
                await Parser.SaveAsync(writer);
            }
            finally
            {
                IsFileWatcherEnabled = lastWatcherState;
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name == _fileName)
                FileChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
        }

        public event EventHandler FileChanged;
    }
}
