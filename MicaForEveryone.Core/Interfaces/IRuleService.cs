using MicaForEveryone.Core.Models;

namespace MicaForEveryone.Core.Interfaces
{
    public interface IRuleService : IDisposable
    {
        void StartService();
        void StopService();
        Task MatchAndApplyRuleToAllWindowsAsync();
        void MatchAndApplyRuleToWindow(TargetWindow target);

        TitlebarColorMode SystemTitlebarColorMode { get; set; }
    }
}
