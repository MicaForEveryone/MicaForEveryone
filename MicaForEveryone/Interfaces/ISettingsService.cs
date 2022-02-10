using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MicaForEveryone.Models;

#nullable enable

namespace MicaForEveryone.Interfaces
{
    public interface ISettingsService
    {
        IConfigFile ConfigFile { get; }
        IRule[] Rules { get; }

        void Load();
        void Save();

        Task LoadRulesAsync();

        void RaiseChanged(SettingsChangeType type, IRule? rule);

        event EventHandler<SettingsChangedEventArgs> Changed;
    }
}
