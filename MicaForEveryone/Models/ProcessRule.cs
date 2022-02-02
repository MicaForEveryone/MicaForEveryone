using Microsoft.Extensions.DependencyInjection;
using Vanara.PInvoke;

using MicaForEveryone.Interfaces;
using MicaForEveryone.UI.Models;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Win32;

namespace MicaForEveryone.Models
{
    public class ProcessRule : IRule
    {
        public ProcessRule(string processName)
        {
            ProcessName = processName;
        }

        public string Name => $"Process({ProcessName})";

        public string ProcessName { get; }

        public TitlebarColorMode TitlebarColor { get; set; }

        public BackdropType BackdropPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetProcessName() == ProcessName;
        }

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
