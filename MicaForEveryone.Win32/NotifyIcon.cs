using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public class NotifyIcon : Window
    {
        private const int NOTIFYICON_VERSION_4 = 4;

        private NOTIFYICONDATA _notifyIconData;
        private uint _taskbarCreatedMessage;
        private bool _shown;

        public NotifyIcon()
        {
            Parent = new IntPtr(-3); // message window
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

        public IntPtr Icon { get; set; }

        public void ShowNotifyIcon()
        {
            _shown = true;

            _notifyIconData.uFlags = NIF.NIF_ICON | NIF.NIF_MESSAGE;
            _notifyIconData.hwnd = Handle;
            _notifyIconData.hIcon = Icon;
            if (Title != null)
            {
                _notifyIconData.uFlags |= NIF.NIF_TIP;
                _notifyIconData.szTip = Title;
            }
            _notifyIconData.hIcon = Class.Icon;
            if (!Shell_NotifyIconW(NIM.NIM_ADD, _notifyIconData))
                return;

            _notifyIconData.uTimeoutOrVersion = NOTIFYICON_VERSION_4;
            if (!Shell_NotifyIconW(NIM.NIM_SETVERSION, _notifyIconData))
                return;
        }

        public void HideNotifyIcon()
        {
            _shown = false;
            _ = Shell_NotifyIconW(NIM.NIM_DELETE, _notifyIconData);
        }

        public RECT GetRect()
        {
            var id = new NOTIFYICONIDENTIFIER(Handle, Id);
            Shell_NotifyIconGetRect(id, out var result);
            return result;            
        }

        protected override IntPtr LoadIcon()
        {
            if (Icon == IntPtr.Zero)
            {
                Icon = base.LoadIcon();
            }
            return Icon;
        }

        protected override IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            if (umsg == (uint)WindowMessage.WM_CREATE)
            {
                _taskbarCreatedMessage = RegisterWindowMessageW("TaskbarCreated");
                if (_taskbarCreatedMessage == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            else if (umsg == _taskbarCreatedMessage && _shown)
            {
                ShowNotifyIcon();
            }
            else if (umsg == CallbackMessage)
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

                    case (ushort)NIN.NIN_SELECT:
                    case (ushort)NIN.NIN_KEYSELECT:
                    case (ushort)WindowMessage.WM_LBUTTONUP:
                        Click?.Invoke(this,
                            new TrayIconClickEventArgs(
                                new Point(
                                    Macros.GET_X_LPARAM(wParam),
                                    Macros.GET_Y_LPARAM(wParam))));
                        break;

                    case (ushort)NIN.NIN_POPUPOPEN:
                        if (OpenPopup == null) break;
                        OpenPopup.Invoke(this, EventArgs.Empty);
                        return IntPtr.Zero;

                    case (ushort)NIN.NIN_POPUPCLOSE:
                        if (ClosePopup == null) break;
                        ClosePopup.Invoke(this, EventArgs.Empty);
                        return IntPtr.Zero;
                }
            }

            return DefWindowProcW(hwnd, umsg, wParam, lParam);
        }

        public event EventHandler<TrayIconClickEventArgs> ContextMenu;
        public event EventHandler<TrayIconClickEventArgs> Click;
        public event EventHandler OpenPopup;
        public event EventHandler ClosePopup;
    }
}
