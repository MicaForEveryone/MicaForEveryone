using System;

using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public class Dialog : Window
    {
        private const int DLGWINDOWEXTRA = 30;

        public Dialog()
        {
            Style = WindowStyles.WS_DLGFRAME;
        }

        protected override void RegisterClass()
        {
            Class = new WindowClass(InstanceHandle, $"{GetType().Name}+{Guid.NewGuid()}", WndProc, IntPtr.Zero, 0, DLGWINDOWEXTRA);
        }

        public override void Close()
        {
            Close(MB_RESULT.IDOK);
        }

        /// <summary>
        /// Close dialog with given result
        /// </summary>
        public void Close(MB_RESULT result)
        {
            EndDialog(Handle, (IntPtr)result);
            DestroyWindow(Handle);
        }

        protected override IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch ((WindowMessage)umsg)
            {
                case WindowMessage.WM_CREATE:
                    var parent = Parent == IntPtr.Zero ?
                        GetDesktopWindow() :
                        FromHandle(Parent);
                    CenterToWindowScaled(parent);
                    Handle = hwnd;
                    UpdatePosition();

                    OnCreate(hwnd);
                    break;

                case WindowMessage.WM_DESTROY:
                    OnDestroy(hwnd);
                    break;

                case WindowMessage.WM_SIZE:
                    OnSizeChanged(hwnd);
                    break;

                case WindowMessage.WM_DPICHANGED:
                    OnDpiChanged(hwnd);
                    break;
            }
            return DefDlgProcW(hwnd, umsg, wParam, lParam);
        }
    }
}
