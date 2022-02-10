using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Config.Reflection;
using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Models
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

        [XclParameter]
        public string ProcessName { get; }

        [XclField]
        public TitlebarColorMode TitleBarColor { get; set; }

        [XclField]
        public BackdropType BackdropPreference { get; set; }

        [XclField]
        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(TargetWindow target) =>
            target.ProcessName == ProcessName;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(ISettingsViewModel parent)
        {
            var viewModel = Program.CurrentApp.Container.GetService<IRuleSettingsViewModel>();
            viewModel.ParentViewModel = parent;
            viewModel.InitializeData(this);
            return new RulePaneItem(ProcessName, PaneItemType.Process, viewModel);
        }
    }
}
