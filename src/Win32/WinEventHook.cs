using System;
using System.ComponentModel;
using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public class WinEventHook : Component
    {
        private User32.HWINEVENTHOOK _eventHook = User32.HWINEVENTHOOK.NULL;

        public WinEventHook()
        {
            InitializeComponent();
        }

        public WinEventHook(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Disposed += OnDisposed;
        }

        public void Hook(uint eventMax, uint eventMin)
        {
            _eventHook = User32.SetWinEventHook(
                eventMin,
                eventMax,
                HINSTANCE.NULL,
                OnHookCallback,
                0,
                0,
                User32.WINEVENT.WINEVENT_OUTOFCONTEXT);
            if (_eventHook.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public void Unhook()
        {
            if (_eventHook.IsNull) return;
            User32.UnhookWinEvent(_eventHook);
            _eventHook = User32.HWINEVENTHOOK.NULL;
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
