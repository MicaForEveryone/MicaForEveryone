using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.App.Services;

public sealed class RuleService : IRuleService
{
    private readonly ISettingsService _settingsService;

    public RuleService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    private static int _currentSession;

    public async Task ApplyRulesToAllWindows()
    {
        // Switch to a background thread, if we are not already in one.
        await TaskScheduler.Default;

        // Increase the session count to prevent concurrency issues,
        // that is, if the user changes the settings while we are applying the rules.
        // This tells the existing procedure to cancel the existing operation.
        int incrementedValue = Interlocked.Increment(ref _currentSession);

        unsafe
        {
            EnumWindows(&EnumWindowsProc, new(incrementedValue));
        }
    }

    [UnmanagedCallersOnly]
    private static BOOL EnumWindowsProc(HWND hWnd, LPARAM lParam)
    {
        if (Volatile.Read(ref _currentSession) != lParam.Value.ToInt32())
            // User changed the settings, cancel the operation.
            return BOOL.FALSE;

        App.Services.GetRequiredService<IRuleService>().ApplyRuleToWindow(hWnd);

        return BOOL.TRUE;
    }

    private static bool IsWindowEligible(HWND hWnd)
    {
        if (!IsWindowVisible(hWnd))
            return false;

        WindowStylesEx styleEx = (WindowStylesEx)GetWindowLongPtrW(hWnd, WindowLongIndex.GWL_EXSTYLE);

        if (styleEx.HasFlag(WindowStylesEx.WS_EX_NOACTIVATE))
            return false;

        if (styleEx.HasFlag(WindowStylesEx.WS_EX_APPWINDOW))
            return true;

        WindowStyles style = (WindowStyles)GetWindowLongPtrW(hWnd, WindowLongIndex.GWL_STYLE);

        bool hasTitleBar = style.HasFlag(WindowStyles.WS_BORDER) && style.HasFlag(WindowStyles.WS_DLGFRAME);

        if (styleEx.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW) && !hasTitleBar)
            return false;

        if (style.HasFlag(WindowStyles.WS_BORDER) & !hasTitleBar)
            return false;

        var isTopLevelWindow = GetAncestor(hWnd, GA_PARENT) == GetDesktopWindow();
        if (!isTopLevelWindow && !styleEx.HasFlag(WindowStylesEx.WS_EX_MDICHILD))
            return false;

        return true;
    }

    public unsafe void ApplyRuleToWindow(HWND hWnd)
    {
        if (!IsWindowEligible(hWnd))
            return;

        Rule mostApplicableRule = _settingsService.Settings!.Rules.Where(f => f.IsRuleApplicable(hWnd)).OrderBy(f => f is not GlobalRule).First();

        if (mostApplicableRule.BackdropPreference != BackdropType.Default)
        {
            uint bp = (uint)mostApplicableRule.BackdropPreference;
            DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, &bp, sizeof(uint));
        }
    }
}
