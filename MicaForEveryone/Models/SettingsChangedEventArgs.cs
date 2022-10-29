using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Models
{
    public enum SettingsChangeType
    {
        RuleAdded,
        RuleRemoved,
        RuleChanged,
        ConfigFilePathChanged,
        ConfigFileWatcherStateChanged,
        ConfigFileReloaded,
        TrayIconVisibilityChanged,
    }

    public class SettingsChangedEventArgs
    {
        public SettingsChangedEventArgs(SettingsChangeType type, IRule? rule)
        {
            Type = type;
            Rule = rule;
        }

        public IRule? Rule { get; }
        public SettingsChangeType Type { get; }
    }
}
