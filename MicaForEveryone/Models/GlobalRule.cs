using Microsoft.Extensions.DependencyInjection;
using XclParser.Reflection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Models
{
    [XclType(TypeName = "Global")]
    public class GlobalRule : IRule
    {
        public string Name => "Global";

        public int Priority => 2;

        [XclField]
        public TitlebarColorMode TitleBarColor { get; set; }

        [XclField]
        public BackdropType BackdropPreference { get; set; }

        [XclField]
        public CornerPreference CornerPreference { get; set; }

        [XclField]
        public bool ExtendFrameIntoClientArea { get; set; }

        [XclField]
        public bool EnableBlurBehind { get; set; }

        [XclField]
        public string CaptionColor { get; set; } = string.Empty;

        [XclField]
        public string CaptionTextColor { get; set; } = string.Empty;

        [XclField]
        public string BorderColor { get; set; } = string.Empty;

        public bool IsApplicable(TargetWindow target) => true;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(UI.ViewModels.ISettingsViewModel parent)
        {
            var viewModel = Program.CurrentApp.Container.GetService<IRuleSettingsViewModel>();
            viewModel.ParentViewModel = parent;
            viewModel.Rule = this;
            return new RulePaneItem("", PaneItemType.Global, viewModel);
        }
    }
}
