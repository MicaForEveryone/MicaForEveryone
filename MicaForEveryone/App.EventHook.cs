using static Vanara.PInvoke.User32;

using MicaForEveryone.Win32;

namespace MicaForEveryone
{
    internal partial class App
    {
        private readonly WinEventHook _eventHook = new(EventConstants.EVENT_OBJECT_CREATE);

        private void InitializeEventHook()
        {
            _eventHook.HookTriggered += EventHook_Triggered;
            _eventHook.Hook();
        }

        private void EventHook_Triggered(object sender, HookTriggeredEventArgs e)
        {
            _ruleHandler.MatchAndApplyRuleToWindow(e.WindowHandle);
        }
    }
}