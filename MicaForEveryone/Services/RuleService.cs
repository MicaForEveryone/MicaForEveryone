using System;
using System.Linq;
using System.Threading.Tasks;
using UIAutomationClient;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.Services
{
    internal class RuleService : IRuleService, IUIAutomationEventHandler
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
        private readonly IUIAutomation _uiAutomation;

        public RuleService(IConfigService configService)
        {
            _configService = configService;
            _configService.ConfigSource.Changed += ConfigSource_Changed;
            _configService.Updated += ConfigService_Updated;

            _uiAutomation = new CUIAutomationClass();
        }

        ~RuleService()
        {
            _configService.ConfigSource.Changed -= ConfigSource_Changed;
        }

        public TitlebarColorMode SystemTitlebarColorMode { get; set; }

        public void StartService()
        {
            _uiAutomation.AddAutomationEventHandler(UIA_EventIds.UIA_Window_WindowOpenedEventId,
                _uiAutomation.GetRootElement(), TreeScope.TreeScope_Children, _uiAutomation.CreateCacheRequest(), this);
        }

        public void StopService()
        {
            // app gets stucked here
            //_uiAutomation.RemoveAllEventHandlers();
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
            var windows = _uiAutomation.GetRootElement().FindAll(TreeScope.TreeScope_Children, _uiAutomation.CreateTrueCondition());
            for (var i = 0; i < windows.Length; i++)
            {
                var window = windows.GetElement(i);
                if (window.CurrentControlType == UIA_ControlTypeIds.UIA_WindowControlTypeId &&
                    Window.ValidateHandle(window.CurrentNativeWindowHandle))
                {
                    var target = TargetWindow.FromAutomationElement(window);
                    MatchAndApplyRuleToWindow(target);
                }
            }
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

        async void IUIAutomationEventHandler.HandleAutomationEvent(IUIAutomationElement sender, int eventId)
        {
            var window = TargetWindow.FromAutomationElement(sender);
            await Task.Run(() =>
            {
                MatchAndApplyRuleToWindow(window);
            });
        }
    }
}
