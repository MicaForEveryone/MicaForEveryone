using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.Shell32;
using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class NotifyIcon : Window
    {
        private NOTIFYICONDATA _notifyIconData;

        public NotifyIcon()
        {
            ClassName = nameof(NotifyIcon);

            Activated += NotifyIcon_Activated;
        }

        public uint Id { get; set; } = 0;

        public uint CallbackId { get; set; }

        private void NotifyIcon_Activated(object sender, EventArgs args)
        {
            _notifyIconData.uFlags = NIF.NIF_ICON | NIF.NIF_TIP | NIF.NIF_MESSAGE;
            _notifyIconData.hwnd = Handle;
            _notifyIconData.uID = Id;
            _notifyIconData.uCallbackMessage = CallbackId;
            _notifyIconData.hIcon = Icon;
            _notifyIconData.szTip = Title;
            if (!Shell_NotifyIcon(NIM.NIM_ADD, _notifyIconData))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            _notifyIconData.uTimeoutOrVersion = 4;
            if (!Shell_NotifyIcon(NIM.NIM_SETVERSION, _notifyIconData))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public void Deactivate()
        {
            Shell_NotifyIcon(NIM.NIM_DELETE, _notifyIconData);
        }

        public RECT GetRect()
        {
            var id = new NOTIFYICONIDENTIFIER(Handle, Id);
            Shell_NotifyIconGetRect(id, out var result).ThrowIfFailed();
            return result;
        }

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            if (umsg == CallbackId)
            {
                switch ((WindowMessage)Macros.LOWORD(lParam))
                {
                    case WindowMessage.WM_CONTEXTMENU:
                        ContextMenu?.Invoke(this, EventArgs.Empty);
                        break;
                    case WindowMessage.WM_LBUTTONUP:
                        Click?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }
            return DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        public event EventHandler ContextMenu;
        public event EventHandler Click;
    }
}
