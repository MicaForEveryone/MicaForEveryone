using System;
using System.Collections.Generic;
using System.Text;
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
