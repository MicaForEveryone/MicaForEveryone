using System;
using System.ComponentModel;
using Vanara.PInvoke;

namespace MicaForEveryone
{
    public class WinEventHook : Component
    {
        private User32.HWINEVENTHOOK hook = User32.HWINEVENTHOOK.NULL;

        public WinEventHook(IContainer container)
        {
            Disposed += OnDisposed;
            container.Add(this);
        }

        public void Hook(uint eventMax, uint eventMin)
        {
            hook = User32.SetWinEventHook(
                eventMin,
                eventMax,
                HINSTANCE.NULL,
                OnHookCallback,
                0,
                0,
                User32.WINEVENT.WINEVENT_OUTOFCONTEXT);
        }

        public void Unhook()
        {
            if (hook.IsNull) return;
            User32.UnhookWinEvent(hook);
            hook = User32.HWINEVENTHOOK.NULL;
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            Unhook();
        }

        protected virtual void OnHookCallback(User32.HWINEVENTHOOK hwineventhook, uint winevent, HWND hwnd, int idobject, int idchild, uint ideventthread, uint dwmseventtime)
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

    public class HookTriggeredEventArgs : EventArgs
    {
        public uint Event { get; set; }
        public HWND WindowHandle { get; set; }
        public int ObjectId { get; set; }
        public int ChildId { get; set; }
    }
}
