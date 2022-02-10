using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Config.Reflection;
using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Models
{
    [XclType(TypeName = "Global")]
    public class GlobalRule : IRule
    {
        public string Name => "Global";

        [XclField]
        public TitlebarColorMode TitleBarColor { get; set; }

        [XclField]
        public BackdropType BackdropPreference { get; set; }

        [XclField]
        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(TargetWindow target) => true;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(ISettingsViewModel parent)
        {
            var viewModel = Program.CurrentApp.Container.GetService<IRuleSettingsViewModel>();
            viewModel.ParentViewModel = parent;
            viewModel.InitializeData(this);
            return new RulePaneItem("", PaneItemType.Global, viewModel);
        }
    }
}
