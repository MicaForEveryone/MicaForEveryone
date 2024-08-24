using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.CoreUI;

public interface IRuleService
{
    void Initialize();

    Task ApplyRulesToAllWindowsAsync();

    Task ApplyRuleToWindowAsync(HWND hwnd);
}
