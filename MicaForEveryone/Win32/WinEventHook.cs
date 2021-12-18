using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class WinEventHook : IDisposable
    {
        private readonly uint _eventId;
        private readonly WinEventProc _eventCallback;

        private HWINEVENTHOOK _eventHook = HWINEVENTHOOK.NULL;

        public WinEventHook(uint eventId)
        {
            _eventId = eventId;
            _eventCallback = OnHookCallback;
        }

        public void Hook()
        {
            _eventHook = SetWinEventHook(
                _eventId,
                _eventId,
                HINSTANCE.NULL,
                _eventCallback,
                0,
                0,
                WINEVENT.WINEVENT_OUTOFCONTEXT);
            if (_eventHook.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public void Unhook()
        {
            if (_eventHook.IsNull) return;
            UnhookWinEvent(_eventHook);
            _eventHook = HWINEVENTHOOK.NULL;
        }

        public void Dispose()
        {
            Unhook();
        }

        private void OnHookCallback(HWINEVENTHOOK hwineventhook, uint winevent, HWND hwnd, int idobject, int idchild, uint ideventthread, uint dwmseventtime)
        {
            HookTriggered?.Invoke(this, new HookTriggeredEventArgs
            {
                Event = winevent,
                WindowHandle = hwnd,
                ObjectId = idobject,
                ChildId = idobject,
            });
        }

        public event EventHandler<HookTriggeredEventArgs> HookTriggered;
    }
}
