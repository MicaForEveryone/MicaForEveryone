using MicaForEveryone.Core.Models;

namespace MicaForEveryone.Core.Interfaces
{
    public interface IRuleService : IDisposable
    {
        void StartService();
        void StopService();
        Task MatchAndApplyRuleToAllWindowsAsync();
        void MatchAndApplyRuleToWindow(TargetWindow target);

        bool IsRunning { get; }
        TitlebarColorMode SystemTitlebarColorMode { get; set; }
    }
}
