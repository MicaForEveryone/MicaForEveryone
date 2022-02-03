using MicaForEveryone.Models;

namespace MicaForEveryone.Interfaces
{
    public interface IRuleService
    {
        void StartService();
        void StopService();
        void MatchAndApplyRuleToAllWindows();
        void MatchAndApplyRuleToWindow(TargetWindow target);

        TitlebarColorMode SystemTitlebarColorMode { get; set; }
    }
}
