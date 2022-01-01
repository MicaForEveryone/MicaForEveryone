using System;
using System.Collections.Generic;
using System.Text;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Services
{
    public class EventHookService : IEventHookService, IDisposable
    {
        private readonly WinEventHook _eventHook = new(EventConstants.EVENT_OBJECT_CREATE);
        private readonly IRuleService _ruleService;

        public EventHookService(IRuleService ruleService)
        {
            _ruleService = ruleService;
            _eventHook.HookTriggered += EventHook_Triggered;
        }

        public void Start()
        {
            _eventHook.Hook();
        }

        public void Dispose()
        {
            _eventHook.Dispose();
        }

        private void EventHook_Triggered(object sender, HookTriggeredEventArgs e)
        {
            _ruleService.MatchAndApplyRuleToWindow(e.WindowHandle);
        }
    }
}
