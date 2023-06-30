using System;
using System.Threading.Tasks;

namespace MicaForEveryone.Interfaces
{
    public interface IStartupService : IDisposable
    {
        Task InitializeAsync();
        Task<bool> SetStateAsync(bool state);

        bool IsEnabled { get; }
        bool IsAvailable { get; }
    }
}
