using System;
using System.Diagnostics;
using System.Linq;
using Vanara.PInvoke;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

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
            var rule = _configService.Rules.Where(rule => rule.IsApplicable(windowHandle)).ToList();
            if (!rule.Any())
                return;

            var globalRule = rule.FirstOrDefault(f => f is GlobalRule);
            if (globalRule != null)
            {
                // Move the global rule to the bottom.
                rule.Remove(globalRule);
                rule.Add(globalRule);
            }

            ApplyRuleToWindow(windowHandle, rule.First());
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            User32.EnumWindows(_enumWindows, IntPtr.Zero);
        }
    }
}
