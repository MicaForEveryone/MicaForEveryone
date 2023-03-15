using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Core.Ui.Models;
using MicaForEveryone.Core.Ui.ViewModels;
using XclParser.Reflection;

namespace MicaForEveryone.Core.Models
{
    [XclType(TypeName = "Process")]
    public class ProcessRule : IRule
    {
        [XclConstructor]
        public ProcessRule(string processName)
        {
            ProcessName = processName;
        }

        public string Name => $"Process({ProcessName})";

        public int Priority => 1;

        [XclParameter]
        public string ProcessName { get; }

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

        public bool IsApplicable(TargetWindow target) =>
            target.ProcessName == ProcessName;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(ISettingsViewModel parent, IRuleSettingsViewModel viewModel)
        {
            viewModel.ParentViewModel = parent;
            viewModel.Rule = this;
            return new RulePaneItem(ProcessName, PaneItemType.Process, viewModel);
        }
    }
}
