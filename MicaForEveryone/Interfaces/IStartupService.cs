using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MicaForEveryone.Interfaces
{
    public interface IStartupService
    {
        Task<bool> SetEnabledAsync(bool state);
        Task<bool> GetEnabledAsync();
    }
}
