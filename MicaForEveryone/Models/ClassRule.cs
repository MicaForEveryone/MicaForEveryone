using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.Models
{
    public class ClassRule : IRule
    {
        public ClassRule(string className)
        {
            ClassName = className;
        }

        public string Name => $"Class({ClassName})";

        public string ClassName { get; }

        public TitlebarColorMode TitlebarColor { get; set; }

        public BackdropType BackdropPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(TargetWindow target) =>
            target.ClassName == ClassName;

        public override string ToString() => Name;

        public RulePaneItem GetPaneItem(ISettingsViewModel parent)
        {
            var viewModel = Program.CurrentApp.Container.GetService<IRuleSettingsViewModel>();
            viewModel.ParentViewModel = parent;
            viewModel.InitializeData(this);
            return new RulePaneItem(ClassName, PaneItemType.Class, viewModel);
        }
    }
}
