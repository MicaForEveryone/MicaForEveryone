using System;
using System.Linq;
using Vanara.PInvoke;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

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
            _enumWindows = (windowHandle, _) =>
            {
                MatchAndApplyRuleToWindow(windowHandle);
                return true;
            };
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
    }
}
