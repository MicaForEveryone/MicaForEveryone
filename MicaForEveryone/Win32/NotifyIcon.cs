using System;
using System.Drawing;
using Vanara.PInvoke;

using static Vanara.PInvoke.Shell32;
using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class NotifyIcon : Window
    {
        private const uint NIN_SELECT = WM_USER + 0;
        private const uint NIN_KEYSELECT = NIN_SELECT | 0x1;
        private const uint NIN_POPUPOPEN = WM_USER + 6;
        private const uint NIN_POPUPCLOSE = WM_USER + 7;

        private NOTIFYICONDATA _notifyIconData;

        public NotifyIcon()
        {
            ClassName = nameof(NotifyIcon);

            Activated += NotifyIcon_Activated;
        }

        public uint Id
        {
            get => _notifyIconData.uID;
            set => _notifyIconData.uID = value;
        }

        public uint CallbackMessage
        {
            get => _notifyIconData.uCallbackMessage;
            set => _notifyIconData.uCallbackMessage = value;
        }

        private void NotifyIcon_Activated(object sender, EventArgs args)
        {
            _notifyIconData.uFlags = NIF.NIF_ICON | NIF.NIF_MESSAGE;
            _notifyIconData.hwnd = Handle;
            if (Title != null)
            {
                _notifyIconData.uFlags |= NIF.NIF_TIP;
                _notifyIconData.szTip = Title;
            }
            _notifyIconData.hIcon = Icon;
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
            if (umsg == CallbackMessage)
            {
                switch (Macros.LOWORD(lParam))
                {
                    case (ushort)WindowMessage.WM_CONTEXTMENU:
                        ContextMenu?.Invoke(this,
                            new TrayIconClickEventArgs(
                                new Point(
                                    Macros.GET_X_LPARAM(wParam),
                                    Macros.GET_Y_LPARAM(wParam))));
                        break;

                    case (ushort)NIN_SELECT:
                    case (ushort)NIN_KEYSELECT:
                    case (ushort)WindowMessage.WM_LBUTTONUP:
                        Click?.Invoke(this, 
                            new TrayIconClickEventArgs(
                                new Point(
                                    Macros.GET_X_LPARAM(wParam),
                                    Macros.GET_Y_LPARAM(wParam))));
                        break;

                    case (ushort)NIN_POPUPOPEN:
                        if (OpenPopup == null) break;
                        OpenPopup.Invoke(this, EventArgs.Empty);
                        return IntPtr.Zero;

                    case (ushort)NIN_POPUPCLOSE:
                        if (ClosePopup == null) break;
                        ClosePopup.Invoke(this, EventArgs.Empty);
                        return IntPtr.Zero;
                }
            }
            return DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        public event EventHandler<TrayIconClickEventArgs> ContextMenu;
        public event EventHandler<TrayIconClickEventArgs> Click;
        public event EventHandler OpenPopup;
        public event EventHandler ClosePopup;
    }

    public class TrayIconClickEventArgs : EventArgs
    {
        public TrayIconClickEventArgs(Point point)
        {
            Point = point;
        }

        public Point Point { get; }
    }
}
