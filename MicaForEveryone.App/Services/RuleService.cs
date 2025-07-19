// ... (existing using statements)
using static TerraFX.Interop.Windows.Windows;

namespace MicaForEveryone.App.Services;

// ... (existing attributes and structs)

public sealed class RuleService : IRuleService
{
    // ... (existing fields)
    private readonly ISettingsService _settingsService;
    private readonly IThemingService _themingService;
    private HWINEVENTHOOK _eventHookHandler;
    
    public bool AreEffectsEnabled { get; set; } = true;

    // ... (existing properties)

    public RuleService(ISettingsService settingsService, IThemingService themingService)
    {
        // ... (existing constructor logic)
    }

    // ... (existing methods)

    public Task ApplyRuleToWindowAsync(HWND hWnd)
    {
        if (!IsWindowEligible(hWnd))
            return Task.CompletedTask;

        // If effects are disabled, revert to default and return
        if (!AreEffectsEnabled)
        {
            // Revert backdrop
            uint defaultBackdrop = (uint)BackdropType.Default;
            DwmSetWindowAttribute(hWnd, 38, &defaultBackdrop, sizeof(uint)); // DWMWA_SYSTEMBACKDROP_TYPE

            // Revert corners
            uint defaultCorners = (uint)CornerPreference.Default;
            DwmSetWindowAttribute(hWnd, 33, &defaultCorners, sizeof(uint)); // DWMWA_WINDOW_CORNER_PREFERENCE
            
            // Revert title bar color
            uint useImmersiveDarkMode = (uint)(_themingService.IsDarkMode() ? 1 : 0);
            DwmSetWindowAttribute(hWnd, 20, &useImmersiveDarkMode, sizeof(uint)); // DWMWA_USE_IMMERSIVE_DARK_MODE
            return Task.CompletedTask;
        }

        Rule mostApplicableRule = _settingsService.Settings!.Rules.Where(f => f.IsRuleApplicable(hWnd)).OrderByDescending(x => x.Priority).First();
        
        // ... (the rest of the method remains the same)
    }
}
