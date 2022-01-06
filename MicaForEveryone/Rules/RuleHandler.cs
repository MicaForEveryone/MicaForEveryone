using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vanara.PInvoke;

using MicaForEveryone.Extensions;
using MicaForEveryone.Models;

namespace MicaForEveryone.Rules
{
    public class RuleHandler
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

        private readonly User32.EnumWindowsProc _enumWindows;

        private bool _isLoading;

        public RuleHandler()
        {
            _enumWindows = (windowHandle, _) =>
            {
                MatchAndApplyRuleToWindow(windowHandle);
                return true;
            };
        }

        public IConfigSource ConfigSource { get; set; }

        public IList<IRule> Rules { get; } = new List<IRule>();

        public GlobalRule GlobalRule { get; private set; }

        public TitlebarColorMode SystemTitlebarColorMode { get; set; }

        public void LoadConfig()
        {
            if (ConfigSource == null) return;

            try
            {
                _isLoading = true;

                var rules = ConfigSource.ParseRules();

                GlobalRule = null;
                Rules.Clear();
                foreach (var rule in rules)
                {
                    if (rule is GlobalRule global)
                    {
                        if (GlobalRule != null)
                            throw new Exception("duplicate global rule section");
                        GlobalRule = global;
                    }
                    else
                    {
                        Rules.Add(rule);
                    }
                }
            }
            finally
            {
                _isLoading = false;
            }

            if (GlobalRule == null)
                throw new Exception("no global rule");
        }

        public void SaveConfig()
        {
            ConfigSource.OverrideRule(GlobalRule);
        }

        public void MatchAndApplyRuleToWindow(HWND windowHandle)
        {
            if (_isLoading) return;
            try
            {
                if (!GlobalRule.IsApplicable(windowHandle)) return;

                var rule = Rules.FirstOrDefault(rule =>
                {
                    try
                    {
                        return rule.IsApplicable(windowHandle);
                    }
#if DEBUG
                    catch
                    {
                        throw;
                    }
#else
                    catch
                    {
                        // ignore errors
                        return false;
                    }
#endif
                }) ?? GlobalRule;

                ApplyRuleToWindow(windowHandle, rule);
            }
#if DEBUG
            catch
            {
                throw;
            }
#else
            catch
            {
                // ignore errors
            }
#endif
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            User32.EnumWindows(_enumWindows, IntPtr.Zero);
        }
    }
}
