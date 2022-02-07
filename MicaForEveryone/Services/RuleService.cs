using System;
using System.Linq;
using System.Threading.Tasks;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.Events;

namespace MicaForEveryone.Services
{
    internal class RuleService : IRuleService
    {
        public void ApplyRuleToWindow(TargetWindow target, IRule rule)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Applying rule `{rule}` to `{target.Title}` ({target.ClassName}, {target.ProcessName})");
#endif
            if (rule.ExtendFrameIntoClientArea)
                DesktopWindowManager.ExtendFrameIntoClientArea(target.WindowHandle);

            target.ApplyTitlebarColorRule(rule.TitlebarColor, SystemTitlebarColorMode);
            target.ApplyBackdropRule(rule.BackdropPreference);
        }

        private readonly IConfigService _configService;

        public RuleService(IConfigService configService)
        {
            _configService = configService;
            _configService.ConfigSource.Changed += ConfigSource_Changed;
            _configService.Updated += ConfigService_Updated;
        }

        ~RuleService()
        {
            _configService.ConfigSource.Changed -= ConfigSource_Changed;
        }

        public TitlebarColorMode SystemTitlebarColorMode { get; set; }

        public void StartService()
        {
            var winEvent = new WindowOpenedEvent();
            winEvent.Handler += WinEvent_Handler;
            WinEventManager.AddEventHandler(winEvent);
        }

        public void StopService()
        {
            WinEventManager.RemoveAll();
        }

        public void MatchAndApplyRuleToWindow(TargetWindow target)
        {
            try
            {
                var applicableRules = _configService.Rules.Where(rule => rule.IsApplicable(target));
                var rule = applicableRules.FirstOrDefault(rule => rule is not GlobalRule) ??
                    applicableRules.FirstOrDefault();

                if (rule == null)
                    return;

                ApplyRuleToWindow(target, rule);
            }
#if DEBUG
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
#else
            catch
            {
                // ignore
            }
#endif
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            Window.GetDesktopWindow().ForEachChild(window =>
            {
                if (!window.IsVisible())
                    return;

                if (!window.IsWindowPatternValid())
                    return;

                if (window.InstanceHandle == Application.InstanceHandle)
                    return; // ignore windows of current instance

                MatchAndApplyRuleToWindow(TargetWindow.FromWindow(window));
            });
        }

        private async void ConfigSource_Changed(object sender, EventArgs e)
        {
            await _configService.LoadAsync();
        }

        private async void ConfigService_Updated(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                MatchAndApplyRuleToAllWindows();
            });
        }

        private async void WinEvent_Handler(object sender, WinEventArgs e)
        {
            await Task.Run(() =>
            {
                if (e.Window.InstanceHandle == Application.InstanceHandle)
                    return; // ignore windows of current instance

                var target = TargetWindow.FromWindow(e.Window);
                MatchAndApplyRuleToWindow(target);
            });
        }
    }
}
