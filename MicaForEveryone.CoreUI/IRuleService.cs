using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.CoreUI;

public interface IRuleService
{
    void Initialize();

    Task ApplyRulesToAllWindows();

    Task ApplyRuleToWindowAsync(HWND hwnd);
}
