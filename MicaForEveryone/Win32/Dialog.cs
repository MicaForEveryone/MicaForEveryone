using MicaForEveryone.Extensions;
using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class Dialog : Window
    {
        private const int DLGWINDOWEXTRA = 30;

        public Dialog()
        {
            Style = WindowStyles.WS_DLGFRAME;
        }

        public override void Activate()
        {
            WindowClass = new WNDCLASS
            {
                hInstance = Instance,
                lpszClassName = ClassName,
                hIcon = Icon,
                lpfnWndProc = WndProc,
                cbWndExtra = DLGWINDOWEXTRA,
            };
            if (RegisterClass(WindowClass) == 0)
            {
                var error = Kernel32.GetLastError();
                if (error != Win32Error.ERROR_CLASS_ALREADY_EXISTS)
                {
                    error.ThrowIfFailed();
                }
            }

            SafeHandle = CreateWindowEx(
                StyleEx,
                ClassName,
                Title,
                Style,
                X, Y, Width, Height,
                Parent,
                HMENU.NULL,
                Instance);

            if (Handle.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public void CenterToDesktop()
        {
            var desktopWindow = GetDesktopWindow();
            GetWindowRect(desktopWindow, out var desktopWindowRect);
            X = (desktopWindowRect.Width - Width) / 2;
            Y = (desktopWindowRect.Height - Height) / 2;
        }

        public void Show()
        {
            ShowWindow(Handle, ShowWindowCommand.SW_SHOW);
        }

        public override void Close()
        {
            Close(MB_RESULT.IDOK);
        }

        public void Close(MB_RESULT result)
        {
            EndDialog(Handle, (IntPtr)result);
            DestroyWindow(Handle);
        }

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch ((WindowMessage)umsg)
            {
                case WindowMessage.WM_CREATE:
                    OnCreate(hwnd);
                    break;

                case WindowMessage.WM_DESTROY:
                    OnDestroy(hwnd);
                    break;
            }
            return DefDlgProc(hwnd, umsg, wParam, lParam);
        }
    }
}
