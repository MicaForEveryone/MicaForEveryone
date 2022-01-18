using Vanara.PInvoke;

using MicaForEveryone.Models;

namespace MicaForEveryone.Interfaces
{
    public interface IRuleService
    {
        public void MatchAndApplyRuleToAllWindows();

        public void MatchAndApplyRuleToWindow(HWND windowHandle);

        TitlebarColorMode SystemTitlebarColorMode { get; set; }
    }
}
