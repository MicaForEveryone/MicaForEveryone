using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using MicaForEveryone.Core.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class Win32ConfigFile : IConfigFile
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

        private static string GetDefaultConfigFilePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#if NETFRAMEWORK
            return Path.Combine(appData, "Mica For Everyone", "MicaForEveryone.conf");
#else
            return Path.Join(appData, "Mica For Everyone", "MicaForEveryone.conf");
#endif
        }

        public static string GetBundledConfigFilePath()
        {
            var appFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
#if NETFRAMEWORK
            return Path.Combine(appFolder, "MicaForEveryone.conf");
#else
            return Path.Join(appFolder, "MicaForEveryone.conf");
#endif
        }

        private readonly FileSystemWatcher _fileSystemWatcher = new();

        private bool _isFileWatcherEnabled;
        private string? _fileName;

        private bool _isInitialized = false;

        public Win32ConfigFile(IConfigParser parser)
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
                if (_isInitialized == false) return;
                _fileSystemWatcher.EnableRaisingEvents = value;
            }
        }

        public Task InitializeAsync()
        {
            return Task.Run(() =>
            {
                FilePath ??= GetDefaultConfigFilePath();
                _fileName = Path.GetFileName(FilePath);
                var directoryPath = Directory.GetParent(FilePath).FullName;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                if (!File.Exists(FilePath))
                {
                    var bundledConfigFilePath = GetBundledConfigFilePath();
                    if (File.Exists(bundledConfigFilePath))
                    {
                        File.Copy(bundledConfigFilePath, FilePath);
                    }
                }
                _fileSystemWatcher.Path = directoryPath;
                _isInitialized = true;
                _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
            });
        }

        public Task ResetAsync()
        {
            return Task.Run(() =>
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                var bundledConfigFilePath = GetBundledConfigFilePath();
                if (File.Exists(bundledConfigFilePath))
                {
                    File.Copy(bundledConfigFilePath, FilePath, true);
                }
                else
                {
                    File.Delete(FilePath);
                }
                _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
            });
        }

        public async Task<IRule[]> LoadAsync()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            try
            {
                using var stream = await TryOpenFile(FilePath, FileMode.Open, FileAccess.Read);
                using var reader = new StreamReader(stream);
                await Parser.LoadAsync(reader);
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
                using var stream = await TryOpenFile(FilePath, FileMode.Create, FileAccess.Write);
                using var writer = new StreamWriter(stream);
                await Parser.SaveAsync(writer);
            }
            finally
            {
                _fileSystemWatcher.EnableRaisingEvents = IsFileWatcherEnabled;
            }
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs args)
        {
            if (IsFileWatcherEnabled && args.Name == _fileName)
                FileChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
        }

        public event EventHandler? FileChanged;
    }
}
