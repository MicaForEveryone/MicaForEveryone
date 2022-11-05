using MicaForEveryone.Models;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Interfaces
{
    public interface IRule
    {
        string Name { get; }

        int Priority { get; }
        
        bool IsApplicable(TargetWindow target);

        TitlebarColorMode TitleBarColor { get; set; }
        BackdropType BackdropPreference { get; set; }
        CornerPreference CornerPreference { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }
        bool EnableBlurBehind { get; set; }

        RulePaneItem GetPaneItem(UI.ViewModels.ISettingsViewModel parent);
    }
}
