using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class RuleHandler
    {
        public static void ApplyRuleToWindow(HWND windowHandle, IRule rule)
        {
#if DEBUG
            Debug.WriteLine($"Applying rule {rule} to {windowHandle.GetText()} ({windowHandle.GetClassName()}, {windowHandle.GetProcessName()})");
#endif

            switch (rule.Mica)
            {
                case MicaMode.Default:
                    break;
                case MicaMode.ForceMica:
                    windowHandle.SetMica(true);
                    break;
                case MicaMode.ForceNoMica:
                    windowHandle.SetMica(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (rule.Theme)
            {
                case ThemeMode.Default:
                    break;
                case ThemeMode.ForceLight:
                    windowHandle.SetImmersiveDarkMode(false);
                    break;
                case ThemeMode.ForceDark:
                    windowHandle.SetImmersiveDarkMode(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IList<IRule> Rules { get; } = new List<IRule>();

        public GlobalRule GlobalRule { get; } = new(ThemeMode.Default, MicaMode.ForceMica);

        public void MatchAndApplyRuleToWindow(HWND windowHandle)
        {
            if (!GlobalRule.IsApplicable(windowHandle)) return;

            var rule = Rules.FirstOrDefault(rule => rule.IsApplicable(windowHandle)) ?? GlobalRule;
            ApplyRuleToWindow(windowHandle, rule);
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            User32.EnumWindows((windowHandle, param) =>
            {
                MatchAndApplyRuleToWindow(windowHandle);
                return true;
            }, IntPtr.Zero);
        }
    }
}
