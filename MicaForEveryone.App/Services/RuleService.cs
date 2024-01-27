using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.VisualStudio.Threading;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.App.Services;

public sealed class RuleService : IRuleService
{
    private readonly ISettingsService _settingsService;
    public RulesModel? SettingsModel { get; set; }

    public RuleService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task ApplyRulesToAllWindows()
    {
        // Switch to a background thread, if we are not already in one.
        await TaskScheduler.Default;

        unsafe
        {
            EnumWindows(&EnumWindowsProc, new(0));
        }
    }

    [UnmanagedCallersOnly]
    private static BOOL EnumWindowsProc(HWND hWnd, LPARAM lParam)
    {
        return BOOL.TRUE;
    }
}
