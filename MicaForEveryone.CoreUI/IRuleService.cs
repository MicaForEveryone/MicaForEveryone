using MicaForEveryone.Models;
using System.Threading.Tasks;

namespace MicaForEveryone.CoreUI;

public interface IRuleService
{
    // MODIFICATION: Add property to control if effects are globally enabled
    bool AreEffectsEnabled { get; set; }
    void Initialize();
    Task ApplyRulesToAllWindowsAsync();
    Task ApplyRuleToWindowAsync(TerraFX.Interop.Windows.HWND hwnd);
    bool AreMaterialsSupported { get; }
    bool AreAdditionalMaterialsSupported { get; }
    bool AreCornerPreferencesSupported { get; }
    BackdropType[] SupportedBackdropTypes { get; }
}
