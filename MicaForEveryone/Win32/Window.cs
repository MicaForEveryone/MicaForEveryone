using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class Window : IDisposable
    {
        private readonly Kernel32.SafeHINSTANCE _instanceHandle;

        private WNDCLASS _class;
        private SafeHWND _windowHandle;

        public Window()
        {
            _instanceHandle = Kernel32.GetModuleHandle();

            if (_instanceHandle.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public string ClassName { get; set; }

        public string Title { get; set; }

        public HICON Icon { get; set; }

        public HWND Handle => _windowHandle;

        public HWND Parent { get; set; } = HWND.NULL;

        public RECT Size { get; set; }

        public WindowStyles Style { get; set; } = 0;

        public WindowStylesEx StyleEx { get; set; } = 0;

        public void Activate()
        {
            _class = new WNDCLASS
            {
                hInstance = _instanceHandle,
                lpszClassName = ClassName,
                hIcon = Icon,
                lpfnWndProc = WndProc,
            };
            if (RegisterClass(_class) == 0)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            _windowHandle = CreateWindowEx(
                StyleEx,
                _class.lpszClassName,
                Title,
                Style,
                Size.X, Size.Y, Size.Width, Size.Height,
                Parent,
                HMENU.NULL,
                _instanceHandle);

            if (_windowHandle.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            Activated?.Invoke(this, EventArgs.Empty);
        }

        public void Show()
        {
            ShowWindow(Handle, ShowWindowCommand.SW_SHOW);
        }

        public void Close()
        {
            if (!PostMessage(_windowHandle, (uint)WindowMessage.WM_DESTROY))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public void Move(RECT size, bool repaint)
        {
            MoveWindow(Handle, size.X, Size.Y, Size.Width, Size.Height, repaint);
        }

        public virtual void Dispose()
        {
            _ = DestroyWindow(_windowHandle);
            _ = UnregisterClass(_class.lpszClassName, _instanceHandle);

            _windowHandle.Dispose();
            _instanceHandle.Dispose();
        }

        protected virtual IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch ((WindowMessage)umsg)
            {
                case WindowMessage.WM_CREATE:
                    OnCreate(hwnd);
                    break;

                case WindowMessage.WM_DESTROY:
                    OnDestroy();
                    break;
            }
            return DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        private void OnCreate(HWND hwnd)
        {
            Create?.Invoke(this, EventArgs.Empty);
        }

        private void OnDestroy()
        {
            Destroy?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Create;
        public event EventHandler Destroy;
        public event EventHandler Activated;
    }
}
