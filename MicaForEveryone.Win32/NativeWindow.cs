using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class NativeWindow : IDisposable
    {
        private const uint USER_DEFAULT_SCREEN_DPI = 96;

        public NativeWindow()
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

        public float ScaleFactor { get; private set; }

        public float ScaledWidth => Width * ScaleFactor;

        public float ScaledHeight => Height * ScaleFactor;

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

            GetWindowRect(Handle, out var winRect);
            X = winRect.X;
            Y = winRect.Y;
            Width = winRect.Width;
            Height = winRect.Height;

            UpdateScaleFactor();
            UpdateSize();

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

        public void UpdateScaleFactor()
        {
            ScaleFactor = ((float)GetDpiForWindow(Handle)) / USER_DEFAULT_SCREEN_DPI;
        }

        public void UpdateSize()
        {
            SetWindowPos(Handle, HWND.NULL, 0, 0, (int)ScaledWidth, (int)ScaledHeight,
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE);
        }

        public void UpdatePosition()
        {
            SetWindowPos(Handle, HWND.NULL, X, Y, 0, 0,
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE);
        }

        public void SetForegroundWindow()
        {
            User32.SetForegroundWindow(Handle);
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

                case WindowMessage.WM_SIZE:
                    OnSizeChanged(hwnd);
                    break;

                case WindowMessage.WM_DPICHANGED:
                    OnDpiChanged(hwnd);
                    break;
            }
            return DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        protected void OnCreate(HWND hwnd)
        {
            Create?.Invoke(this, new Win32EventArgs(hwnd));
        }

        protected void OnDestroy(HWND hwnd)
        {
            Destroy?.Invoke(this, new Win32EventArgs(hwnd));
        }

        protected void OnSizeChanged(HWND hwnd)
        {
            SizeChanged?.Invoke(this, new Win32EventArgs(hwnd));
        }

        protected void OnDpiChanged(HWND hwnd)
        {
            UpdateScaleFactor();
            UpdateSize();
            DpiChanged?.Invoke(this, new Win32EventArgs(hwnd));
        }

        public event EventHandler<Win32EventArgs> Create;
        public event EventHandler<Win32EventArgs> Destroy;
        public event EventHandler<Win32EventArgs> SizeChanged;
        public event EventHandler<Win32EventArgs> DpiChanged;
    }

    public class Win32EventArgs : EventArgs
    {
        public Win32EventArgs(HWND windowHandle)
        {
            WindowHandle = windowHandle;
        }

        public HWND WindowHandle { get; }
    }
}
