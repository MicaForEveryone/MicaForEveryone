using MicaForEveryone.Core.Models;

namespace MicaForEveryone.Core.Interfaces
{
    public interface ISettingsService : IDisposable
    {
        IRule[] Rules { get; }
        bool TrayIconVisibility { get; set; }
        string? ConfigFilePath { get; set; }
        bool IsFileWatcherEnabled { get; set; }
        //Language Language { get; set; }

        Task InitializeAsync();
        Task AddRuleAsync(IRule rule);
        Task RemoveRuleAsync(IRule rule);
        Task UpdateRuleAsync(IRule rule);
        Task LoadRulesAsync();
        Task ResetRulesAsync();
        
        event EventHandler<RulesChangeEventArgs> RuleAdded;
        event EventHandler<RulesChangeEventArgs> RuleRemoved;
        event EventHandler<RulesChangeEventArgs> RuleChanged;
        event EventHandler ConfigFilePathChanged;
        event EventHandler ConfigFileWatcherStateChanged;
        event EventHandler ConfigFileReloaded;
        event EventHandler TrayIconVisibilityChanged;
        //event EventHandler LanguageChanged;
    }
}
