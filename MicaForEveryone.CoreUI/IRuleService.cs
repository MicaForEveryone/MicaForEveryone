using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.CoreUI;

public interface IRuleService
{
    Task ApplyRulesToAllWindows();

    void ApplyRuleToWindow(HWND hwnd);
}
