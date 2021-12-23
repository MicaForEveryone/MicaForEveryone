using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class Window : IDisposable
    {
        public Window()
        {
            Instance = Kernel32.GetModuleHandle();

            if (Instance.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public Kernel32.SafeHINSTANCE Instance { get; }

        public string ClassName { get; set; }

        public string Title { get; set; }

        public HICON Icon { get; set; }

        public SafeHWND SafeHandle { get; protected set; }

        public HWND Handle => SafeHandle ?? HWND.NULL;

        public HWND Parent { get; set; } = HWND.NULL;

        public int X { get; set; } = CW_USEDEFAULT;
        
        public int Y { get; set; } = CW_USEDEFAULT;

        public int Width { get; set; } = CW_USEDEFAULT;

        public int Height { get; set; } = CW_USEDEFAULT;

        public WindowStyles Style { get; set; } = 0;

        public WindowStylesEx StyleEx { get; set; } = 0;

        protected WNDCLASS WindowClass { get; set; }

        public virtual void Activate()
        {
            if (SafeHandle != null) return;
            WindowClass = new WNDCLASS
            {
                hInstance = Instance,
                lpszClassName = ClassName,
                hIcon = Icon,
                lpfnWndProc = WndProc,
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

        public virtual void Close()
        {
            if (!PostMessage(Handle, (uint)WindowMessage.WM_CLOSE))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public virtual void Dispose()
        {
            _ = DestroyWindow(Handle);
            _ = UnregisterClass(WindowClass.lpszClassName, Instance);

            SafeHandle?.Dispose();
            Instance?.Dispose();
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
