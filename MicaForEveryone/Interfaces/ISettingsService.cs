using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MicaForEveryone.Models;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    public interface ISettingsService : IDisposable
    {
        IConfigFile ConfigFile { get; }
        IRule[] Rules { get; }
        bool TrayIconVisibility { get; set; }

        void Load();
        void Save();

        Task LoadRulesAsync();

        Task CommitChangesAsync(SettingsChangeType type, IRule? rule);

        event EventHandler<SettingsChangedEventArgs> Changed;
    }
}
