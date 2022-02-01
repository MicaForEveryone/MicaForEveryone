using Microsoft.Extensions.DependencyInjection;
using Vanara.PInvoke;

using MicaForEveryone.Win32;
using MicaForEveryone.Interfaces;
using MicaForEveryone.ViewModels;

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

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetClassName() == ClassName;
        }

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
