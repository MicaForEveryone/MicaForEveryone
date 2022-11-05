using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MicaForEveryone.Models;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.Events;

namespace MicaForEveryone.Services
{
    internal class RuleService : IRuleService
    {
        private readonly ISettingsService _settingsService;

        private readonly Mutex _applyAllWindowsMutex = new();

        public RuleService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _settingsService.Changed += SettingsService_Changed;
        }

        ~RuleService()
        {
            Dispose(false);
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
                var applicableRules = _settingsService.Rules.Where(rule => rule.IsApplicable(target)).OrderBy(rule => rule.Priority);
                var rule = applicableRules.FirstOrDefault();

                if (rule == null)
                    return;

                target.ApplyRule(rule, SystemTitlebarColorMode);
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

        public Task MatchAndApplyRuleToAllWindowsAsync()
        {
            return Task.Run(() =>
            {
                _applyAllWindowsMutex.WaitOne();
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
                _applyAllWindowsMutex.ReleaseMutex();
            });

        }

        private void SettingsService_Changed(object sender, SettingsChangedEventArgs args)
        {
            if (args.Type is SettingsChangeType.ConfigFileWatcherStateChanged
                or SettingsChangeType.ConfigFilePathChanged)
                return;

            _ = MatchAndApplyRuleToAllWindowsAsync();
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

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _applyAllWindowsMutex?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
