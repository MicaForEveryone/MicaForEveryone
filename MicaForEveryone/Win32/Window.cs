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

        public virtual void Activate()
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
                var error = Kernel32.GetLastError();
                if (error != Win32Error.ERROR_CLASS_ALREADY_EXISTS)
                {
                    error.ThrowIfFailed();
                }
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
        }

        public void Close()
        {
            if (!PostMessage(_windowHandle, (uint)WindowMessage.WM_CLOSE))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
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
                    OnDestroy(hwnd);
                    break;
            }
            return DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        protected void OnCreate(HWND hwnd)
        {
            Create?.Invoke(this, new WindowEventArgs(hwnd));
        }

        protected void OnDestroy(HWND hwnd)
        {
            Destroy?.Invoke(this, new WindowEventArgs(hwnd));
        }

        public event EventHandler<WindowEventArgs> Create;
        public event EventHandler<WindowEventArgs> Destroy;
    }

    public class WindowEventArgs : EventArgs
    {
        public WindowEventArgs(HWND windowHandle)
        {
            WindowHandle = windowHandle;
        }

        public HWND WindowHandle { get; }
    }
}
