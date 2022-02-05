using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public class Window : IDisposable
    {
        public const int CW_USEDEFAULT = unchecked((int)0x80000000);

        private const uint USER_DEFAULT_SCREEN_DPI = 96;

        public static void AdjustWindowRectEx(ref RECT lpRect, WindowStyles dwStyle, WindowStylesEx dwExStyle)
        {
            if (!NativeMethods.AdjustWindowRectEx(ref lpRect, dwStyle, false, dwExStyle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static Window FromHandle(IntPtr hWnd)
        {
            return new Window
            {
                Handle = hWnd,
            };
        }

        public static bool ValidateHandle(IntPtr hWnd)
        {
            return IsWindow(hWnd);
        }

        public static Window GetDesktopWindow()
        {
            return FromHandle(NativeMethods.GetDesktopWindow());
        }

        private bool _isDisposing = false;

        public string Title { get; set; }

        public IntPtr Handle { get; protected set; }

        public IntPtr Parent { get; set; } = IntPtr.Zero;

        public int X { get; set; } = CW_USEDEFAULT;

        public int Y { get; set; } = CW_USEDEFAULT;

        public int Width { get; set; } = CW_USEDEFAULT;

        public int Height { get; set; } = CW_USEDEFAULT;

        public float ScaleFactor { get; private set; } = 1;

        public float ScaledWidth => Width * ScaleFactor;

        public float ScaledHeight => Height * ScaleFactor;

        public WindowStyles Style { get; set; } = 0;

        public WindowStylesEx StyleEx { get; set; } = 0;

        public WindowClass Class { get; protected set; }

        protected virtual void RegisterClass()
        {
            var module = LoadLibraryW("imageres.dll");
            if (module == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            var icon = LoadIconW(module, "#15");
            if (icon == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            Class = new WindowClass($"{GetType().Name}+{Guid.NewGuid()}", WndProc, icon);
            if (!FreeLibrary(module))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        protected virtual void CreateWindow()
        {
            Handle = CreateWindowExW(
                StyleEx,
                Class.Atom,
                Title,
                Style,
                X, Y, Width, Height,
                Parent,
                IntPtr.Zero,
                InstanceHandle);

            if (Handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual void Activate()
        {
            if (Handle != IntPtr.Zero) return;

            RegisterClass();
            CreateWindow();

            var windowRect = GetWindowRect();
            X = windowRect.X;
            Y = windowRect.Y;
            Width = windowRect.Width;
            Height = windowRect.Height;

            UpdateScaleFactor();
            UpdateSize();
        }

        public void Show()
        {
            Show(ShowWindowCommand.SW_SHOW);
        }

        public virtual void Show(ShowWindowCommand command)
        {
            ShowWindow(Handle, command);
        }

        public virtual void Close()
        {
            if (!PostMessageW(Handle, (uint)WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual void Dispose()
        {
            if (_isDisposing) return;
            _isDisposing = true;
            _ = DestroyWindow(Handle);
            Handle = IntPtr.Zero;
            Class.Dispose();
            _ = DestroyIcon(Class.Icon);
        }

        public void UpdateScaleFactor()
        {
            ScaleFactor = ((float)GetDpiForWindow(Handle)) / USER_DEFAULT_SCREEN_DPI;
        }

        public void UpdateSize()
        {
            SetWindowPosScaled(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE);
        }

        public void UpdatePosition()
        {
            SetWindowPosScaled(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE);
        }

        public void SetForegroundWindow()
        {
            NativeMethods.SetForegroundWindow(Handle);
        }

        public void SetWindowPos(IntPtr hWndInsertAfter, SetWindowPosFlags flags)
        {
            if (!NativeMethods.SetWindowPos(Handle, hWndInsertAfter, X, Y, Width, Height, flags))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public void SetWindowPosScaled(IntPtr hWndInsertAfter, SetWindowPosFlags flags)
        {
            if (!NativeMethods.SetWindowPos(Handle, hWndInsertAfter, X, Y, (int)ScaledWidth, (int)ScaledHeight, flags))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public RECT GetClientRect()
        {
            if (!NativeMethods.GetClientRect(Handle, out var lpRect))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return lpRect;
        }

        public RECT GetWindowRect()
        {
            if (!NativeMethods.GetWindowRect(Handle, out var lpRect))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return lpRect;
        }

        public void CenterToDesktop()
        {
            var desktopWindowRect = GetDesktopWindow().GetWindowRect();
            X = (desktopWindowRect.Width - Width) / 2;
            Y = (desktopWindowRect.Height - Height) / 2;
        }

        public void CenterToDesktopScaled()
        {
            var desktopWindowRect = GetDesktopWindow().GetWindowRect();
            X = (int)((desktopWindowRect.Width - ScaledWidth) / 2);
            Y = (int)((desktopWindowRect.Height - ScaledHeight) / 2);
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
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
            return DefWindowProcW(hwnd, umsg, wParam, lParam);
        }

        protected void OnCreate(IntPtr hwnd)
        {
            Handle = hwnd;
            Create?.Invoke(this, new WndProcEventArgs(hwnd));
        }

        protected void OnDestroy(IntPtr hwnd)
        {
            Destroy?.Invoke(this, new WndProcEventArgs(hwnd));
        }

        protected void OnSizeChanged(IntPtr hwnd)
        {
            SizeChanged?.Invoke(this, new WndProcEventArgs(hwnd));
        }

        protected void OnDpiChanged(IntPtr hwnd)
        {
            UpdateScaleFactor();
            UpdateSize();
            DpiChanged?.Invoke(this, new WndProcEventArgs(hwnd));
        }

        public event EventHandler<WndProcEventArgs> Create;
        public event EventHandler<WndProcEventArgs> Destroy;
        public event EventHandler<WndProcEventArgs> SizeChanged;
        public event EventHandler<WndProcEventArgs> DpiChanged;
    }
}
