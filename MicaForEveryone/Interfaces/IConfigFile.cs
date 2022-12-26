using System;
using System.Threading.Tasks;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    public interface IConfigFile : IDisposable
    {
        IConfigParser Parser { get; }

        string? FilePath { get; set; }
        bool IsFileWatcherEnabled { get; set; }

        Task InitializeAsync();
        Task ResetAsync();

        Task<IRule[]> LoadAsync();
        Task SaveAsync();

        event EventHandler FileChanged;
    }
}
