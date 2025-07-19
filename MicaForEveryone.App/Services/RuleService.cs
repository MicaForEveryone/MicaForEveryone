using MicaForEveryone.App.Helpers;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace MicaForEveryone.App.Services;

// (Existing enums and structs remain the same)

public sealed class RuleService : IRuleService
{
    [DllImport("user32")]
    private static extern BOOL IsTopLevelWindow(HWND hWnd);

    [DllImport("user32")]
    private static unsafe extern BOOL SetWindowCompositionAttribute(HWND hWnd, WINDOWCOMPOSITIONATTRIBDATA* data);

    private readonly ISettingsService _settingsService;
    private readonly IThemingService _themingService;
    private HWINEVENTHOOK _eventHookHandler;

    // MODIFICATION: Property to globally enable/disable effects
    public bool AreEffectsEnabled { get; set; } = true;

    public BackdropType[] SupportedBackdropTypes { get; }

    public RuleService(ISettingsService settingsService, IThemingService themingService)
    {
        _settingsService = settingsService;
        _themingService = themingService;
        _themingService.ThemeChanged += (_, _) => _ = ApplyRulesToAllWindowsAsync();

        if (!AreAdditionalMaterialsSupported)
            SupportedBackdropTypes = new[] { BackdropType.Default, BackdropType.None, BackdropType.Mica };
        else
            SupportedBackdropTypes = Enum.GetValues<BackdropType>();
    }

    Lazy<bool> _is22000 = new(static () => Environment.OSVersion.Version >= new Version(10, 0, 22000));
    Lazy<bool> _is22523 = new(static () => Environment.OSVersion.Version >= new Version(10, 0, 22523));

    public bool AreMaterialsSupported => _is22000.Value;
    public bool AreAdditionalMaterialsSupported => _is22523.Value;
    public bool AreCornerPreferencesSupported => _is22000.Value;

    public unsafe void Initialize()
    {
        _eventHookHandler = SetWinEventHook(EVENT.EVENT_OBJECT_SHOW, EVENT.EVENT_OBJECT_SHOW, HMODULE.NULL, &NewWindowShown, 0, 0, WINEVENT_OUTOFCONTEXT);
        _settingsService.PropertyChanged += _settingsService_PropertyChanged;
    }

    private void _settingsService_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        _ = ApplyRulesToAllWindowsAsync();
    }

    [UnmanagedCallersOnly]
    private static void NewWindowShown(HWINEVENTHOOK handler, uint winEvent, HWND hWnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
    {
        async Task NewWindowShowHandlerAsync(IRuleService service, HWND hwnd)
        {
            if (!IsWindowEligible(hwnd))
                await Task.Delay(10);
            await service.ApplyRuleToWindowAsync(hwnd);
        }

        _ = NewWindowShowHandlerAsync(App.Services.GetRequiredService<IRuleService>(), hWnd);
    }

    private void CallEnumWindows()
    {
        RuleService currentRuleService = this;
        Ref<RuleService> ruleService = new(ref currentRuleService);

        unsafe
        {
            EnumWindows(&EnumWindowsProc, new((nint)Unsafe.AsPointer(ref ruleService)));
        }
    }

    public async Task ApplyRulesToAllWindowsAsync()
    {
        await TaskScheduler.Default;
        CallEnumWindows();
    }

    [UnmanagedCallersOnly]
    private static BOOL EnumWindowsProc(HWND hWnd, LPARAM lParam)
    {
        unsafe
        {
            Unsafe.AsRef<Ref<RuleService>>(lParam).GetReference().ApplyRuleToWindowAsync(hWnd);
        }
        return BOOL.TRUE;
    }
    
    private static unsafe bool IsWindowEligible(HWND hWnd)
    {
        if (!IsWindowVisible(hWnd))
            return false;
        nint styleEx = GetWindowLongPtrW(hWnd, GWL.GWL_EXSTYLE);
        nint style = GetWindowLongPtrW(hWnd, GWL.GWL_STYLE);
        if ((styleEx & WS.WS_EX_NOACTIVATE) == WS.WS_EX_NOACTIVATE || (styleEx & WS.WS_EX_TRANSPARENT) == WS.WS_EX_TRANSPARENT)
            return false;
        if (IsTopLevelWindow(hWnd) == BOOL.FALSE)
            return false;
        bool hasTitleBar = (style & WS.WS_BORDER) == WS.WS_BORDER && (style & WS.WS_DLGFRAME) == WS.WS_DLGFRAME;
        if ((styleEx & WS.WS_EX_TOOLWINDOW) == WS.WS_EX_TOOLWINDOW && !hasTitleBar)
            return false;
        if ((style & WS.WS_POPUP) == WS.WS_POPUP & !hasTitleBar)
            return false;
        return true;
    }

    public unsafe Task ApplyRuleToWindowAsync(HWND hWnd)
    {
        if (!IsWindowEligible(hWnd))
            return Task.CompletedTask;

        // MODIFICATION: Check if effects are disabled and revert if they are
        if (!AreEffectsEnabled)
        {
            // Revert backdrop by setting it to Default (which is 0, but let's be explicit)
            uint defaultBackdrop = (uint)BackdropType.Default; // None is 1, Mica is 2 etc. 0 should be default.
            if (AreAdditionalMaterialsSupported)
            {
                const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
                DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, &defaultBackdrop, sizeof(uint));
            }
            else
            {
                const uint DWMWA_MICA_EFFECT = 1029;
                int micaValue = 0; // disabled
                DwmSetWindowAttribute(hWnd, DWMWA_MICA_EFFECT, &micaValue, sizeof(int));
            }

            // Revert corners
            uint defaultCorners = (uint)CornerPreference.Default;
            const uint DWMWA_CORNER_PREFERENCE = 33;
            DwmSetWindowAttribute(hWnd, DWMWA_CORNER_PREFERENCE, &defaultCorners, sizeof(uint));
            
            // Revert title bar color
            const uint DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
            uint useImmersiveDarkMode = (uint)(_themingService.IsDarkMode() ? 1 : 0);
            DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, &useImmersiveDarkMode, sizeof(uint));
            
            return Task.CompletedTask;
        }

        Rule mostApplicableRule = _settingsService.Settings!.Rules.Where(f => f.IsRuleApplicable(hWnd)).OrderByDescending(x => x.Priority).First();

        const uint DWMWA_CAPTION_COLOR = 35;
        switch (mostApplicableRule.TitleBarColor)
        {
            case TitleBarColorMode.System:
            case TitleBarColorMode.Light:
            case TitleBarColorMode.Dark:
                const uint DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
                TitleBarColorMode normalizedTitleBarColorMode = mostApplicableRule.TitleBarColor == TitleBarColorMode.System ? _themingService.IsDarkMode() ? TitleBarColorMode.Dark : TitleBarColorMode.Light : mostApplicableRule.TitleBarColor;
                uint useImmersiveDarkMode = (uint)(normalizedTitleBarColorMode == TitleBarColorMode.Dark ? 1 : 0);
                DwmSetWindowAttribute(hWnd, DWMWA_USE_IMMERSIVE_DARK_MODE, &useImmersiveDarkMode, sizeof(uint));
                uint defaultCaption = DWMWA_COLOR_DEFAULT;
                DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, &defaultCaption, sizeof(uint));
                break;
            case TitleBarColorMode.Custom:
                Windows.UI.Color color = ColorConverter.ConvertToColor(mostApplicableRule.TitleBarColorCode);
                COLORREF colorref = RGB(color.R, color.G, color.B);
                DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, &colorref, (uint)sizeof(COLORREF));
                break;
        }

        if (mostApplicableRule.BackdropPreference != BackdropType.Default)
        {
            uint bp = (uint)mostApplicableRule.BackdropPreference;
            if (AreAdditionalMaterialsSupported)
            {
                const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
                DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, &bp, sizeof(uint));
            }
            else
            {
                const uint DWMWA_MICA_EFFECT = 1029;
                int micaValue = mostApplicableRule.BackdropPreference == BackdropType.Mica ? 1 : 0;
                DwmSetWindowAttribute(hWnd, DWMWA_MICA_EFFECT, &micaValue, sizeof(int));
            }
        }

        if (mostApplicableRule.CornerPreference != CornerPreference.Default)
        {
            uint cp = (uint)mostApplicableRule.CornerPreference;
            const uint DWMWA_CORNER_PREFERENCE = 33;
            DwmSetWindowAttribute(hWnd, DWMWA_CORNER_PREFERENCE, &cp, sizeof(uint));
        }

        if (mostApplicableRule.ExtendFrameIntoClientArea)
        {
            MARGINS margins = new() { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
            DwmExtendFrameIntoClientArea(hWnd, &margins);
        }

        if (mostApplicableRule.EnableBlurBehind)
        {
            DWM_BLURBEHIND bb = new() { fEnable = BOOL.TRUE, dwFlags = DWM.DWM_BB_ENABLE, fTransitionOnMaximized = BOOL.FALSE, hRgnBlur = HRGN.NULL };
            DwmEnableBlurBehindWindow(hWnd, &bb);

            ACCENT_POLICY accent = new() { AccentState = ACCENT_STATE.ACCENT_ENABLE_BLURBEHIND | ACCENT_STATE.ACCENT_ENABLE_GRADIENT, GradientColor = unchecked((uint)((152 << 24) | (0x2B2B2B & 0xFFFFFF))) };
            WINDOWCOMPOSITIONATTRIBDATA attrib = new() { Attrib = WINDOWCOMPOSITIONATTRIB.WCA_ACCENT_POLICY, pvData = (nint)(&accent), cbData = (uint)sizeof(ACCENT_POLICY) };
            SetWindowCompositionAttribute(hWnd, &attrib);
        }

        return Task.CompletedTask;
    }
}
