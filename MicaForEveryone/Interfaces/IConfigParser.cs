using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MicaForEveryone.Interfaces
{
    public interface IConfigParser
    {
        IRule[] Rules { get; }

        Task LoadAsync(StreamReader path);
        Task SaveAsync(StreamWriter path);

        void AddRule(IRule rule);
        void SetRule(IRule rule);
        void RemoveRule(IRule rule);
    }
}
