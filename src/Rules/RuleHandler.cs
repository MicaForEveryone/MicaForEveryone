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
            Debug.WriteLine($"Applying rule `{rule}` to `{windowHandle.GetText()}` ({windowHandle.GetClassName()}, {windowHandle.GetProcessName()})");
#endif
            if (rule.ExtendFrameIntoClientArea)
                windowHandle.ExtendFrameIntoClientArea();

            switch (rule.MicaPreference)
            {
                case MicaPreference.Default:
                    break;
                case MicaPreference.PreferEnabled:
                    windowHandle.SetMica(true);
                    break;
                case MicaPreference.PreferDisabled:
                    windowHandle.SetMica(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (rule.TitlebarColor)
            {
                case TitlebarColorMode.Default:
                    break;
                case TitlebarColorMode.Light:
                    windowHandle.SetImmersiveDarkMode(false);
                    break;
                case TitlebarColorMode.Dark:
                    windowHandle.SetImmersiveDarkMode(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IConfigSource ConfigSource { get; set; }

        public IList<IRule> Rules { get; } = new List<IRule>();

        public GlobalRule GlobalRule { get; private set; }

        public void LoadConfig()
        {
            if (ConfigSource == null) return;
            Rules.Clear();
            GlobalRule = ConfigSource.GetGlobalRule();
            foreach (var rule in ConfigSource.ParseRules())
            {
                Rules.Add(rule);
            }
        }

        public void MatchAndApplyRuleToWindow(HWND windowHandle)
        {
            if (!GlobalRule.IsApplicable(windowHandle)) return;

            var rule = Rules.FirstOrDefault(rule => rule.IsApplicable(windowHandle)) ?? GlobalRule;
            ApplyRuleToWindow(windowHandle, rule);
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            User32.EnumWindows(OnEnumerateWindow, IntPtr.Zero);
        }

        private bool OnEnumerateWindow(HWND windowHandle, IntPtr param)
        {
            MatchAndApplyRuleToWindow(windowHandle);
            return true;
        }
    }
}
