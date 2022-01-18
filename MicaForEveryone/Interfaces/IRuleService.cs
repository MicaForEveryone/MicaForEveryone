using Vanara.PInvoke;

namespace MicaForEveryone.Interfaces
{
    public interface IRuleService
    {
        public void MatchAndApplyRuleToAllWindows();

        public void MatchAndApplyRuleToWindow(HWND windowHandle);
    }
}
