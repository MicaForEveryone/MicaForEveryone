using System;
using System.Linq;
using Vanara.PInvoke;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.ViewModels;

#if DEBUG
using System.Diagnostics;
#endif

namespace MicaForEveryone.Services
{
    internal class RuleHandler : IRuleService
    {
        public void ApplyRuleToWindow(HWND windowHandle, IRule rule)
        {
#if DEBUG
            Debug.WriteLine($"Applying rule `{rule}` to `{windowHandle.GetText()}` ({windowHandle.GetClassName()}, {windowHandle.GetProcessName()})");
#endif
            if (rule.ExtendFrameIntoClientArea)
                windowHandle.ExtendFrameIntoClientArea();

            windowHandle.ApplyBackdropRule(rule.BackdropPreference);
            windowHandle.ApplyTitlebarColorRule(rule.TitlebarColor, SystemTitlebarColorMode);
        }

        private readonly IConfigService _configService;
        private readonly User32.EnumWindowsProc _enumWindows;

        public RuleHandler(IConfigService configService)
        {
            _configService = configService;
            _configService.ConfigSource.Updated += ConfigSource_Updated;
            _enumWindows = (windowHandle, _) =>
            {
                if (!windowHandle.IsOwned())
                    MatchAndApplyRuleToWindow(windowHandle);
                return true;
            };
        }

        ~RuleHandler()
        {
            _configService.ConfigSource.Updated -= ConfigSource_Updated;
        }

        public TitlebarColorMode SystemTitlebarColorMode { get; set; }

        public void MatchAndApplyRuleToWindow(HWND windowHandle)
        {
            var applicableRules = _configService.Rules.Where(rule => rule.IsApplicable(windowHandle));

            if (applicableRules.All(rule => rule is not GlobalRule))
                return;

            var rule = applicableRules.FirstOrDefault(rule => rule is not GlobalRule) ??
                applicableRules.First();

            ApplyRuleToWindow(windowHandle, rule);
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            User32.EnumWindows(_enumWindows, IntPtr.Zero);
        }

        private void ConfigSource_Updated(object sender, EventArgs e)
        {
            var viewService = Program.CurrentApp.Container.GetService<IViewService>();
            viewService.MainWindow.RequestReloadConfig();
        }
    }
}
