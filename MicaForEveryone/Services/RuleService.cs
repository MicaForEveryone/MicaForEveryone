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
        private readonly ILogger _loggerService;

        private readonly Mutex _applyAllWindowsMutex = new();

        public RuleService(ISettingsService settingsService, ILogger loggerService)
		{
			_settingsService = settingsService;
			_settingsService.Changed += SettingsService_Changed;
			_loggerService = loggerService;
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

                _loggerService.Add(new LogEntry(true, target.Title, target.ClassName, target.ProcessName, ""));
            }
#if DEBUG
            catch (Exception ex)
            {
				_loggerService.Add(new LogEntry(false, target.Title, target.ClassName, target.ProcessName, ex.Message));
				System.Diagnostics.Debug.WriteLine(ex);
				System.Diagnostics.Debugger.Break();
            }
#else
            catch
            {
                _loggerService.Add(new LogEntry(false, target.Title, target.ClassName, target.ProcessName));
                // ignore
            }
#endif
		}

		public Task MatchAndApplyRuleToAllWindowsAsync()
        {
            return Task.Run(() =>
            {
                try
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
                }
                finally
                {
                    _applyAllWindowsMutex.ReleaseMutex();
                }
            });

        }

        private void SettingsService_Changed(object sender, SettingsChangedEventArgs args)
        {
            if (args.Type is SettingsChangeType.ConfigFileWatcherStateChanged
                or SettingsChangeType.ConfigFilePathChanged)
                return;

            _ = MatchAndApplyRuleToAllWindowsAsync();
        }

        private void WinEvent_Handler(object sender, WindowOpenedEventArgs args)
        {
			if (args.Window.InstanceHandle == Application.InstanceHandle)
				return; // ignore windows of current instance

			var target = TargetWindow.FromWindow(args.Window);
			MatchAndApplyRuleToWindow(target);
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
