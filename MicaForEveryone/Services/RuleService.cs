using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

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
                target.WindowHandle.ExtendFrameIntoClientArea();

            target.WindowHandle.ApplyBackdropRule(rule.BackdropPreference);
            target.WindowHandle.ApplyTitlebarColorRule(rule.TitlebarColor, SystemTitlebarColorMode);
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
            Automation.AddAutomationEventHandler(
                WindowPattern.WindowOpenedEvent,
                AutomationElement.RootElement,
                TreeScope.Children,
                Automation_WindowOpened);
        }

        public void StopService()
        {
            Automation.RemoveAllEventHandlers();
        }

        public void MatchAndApplyRuleToWindow(TargetWindow window)
        {
            var applicableRules = _configService.Rules.Where(rule => rule.IsApplicable(window));
            var rule = applicableRules.FirstOrDefault(rule => rule is not GlobalRule) ??
                applicableRules.FirstOrDefault();

            if (rule == null)
                return;

            ApplyRuleToWindow(window, rule);
        }

        public void MatchAndApplyRuleToAllWindows()
        {
            var windows = AutomationElement.RootElement.FindAll(TreeScope.Children, Condition.TrueCondition);
            foreach (AutomationElement window in windows)
            {
                if (window.Current.ControlType == ControlType.Window)
                {
                    var target = TargetWindow.FromAutomationElement(window);
                    MatchAndApplyRuleToWindow(target);
                }
            }
        }

        private async void Automation_WindowOpened(object sender, AutomationEventArgs args)
        {
            var window = TargetWindow.FromAutomationElement((AutomationElement)sender);
            await Task.Run(() =>
            {
                MatchAndApplyRuleToWindow(window);
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
    }
}
