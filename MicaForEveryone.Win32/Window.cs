using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32
{
    public class Window : IDisposable
    {
        public const int CW_USEDEFAULT = unchecked((int)0x80000000);

        private const uint ERROR_MOD_NOT_FOUND = 0x0000007E;
        private const uint USER_DEFAULT_SCREEN_DPI = 96;
        private const int GWL_STYLE = -16;
        private const int GWL_EXSTYLE = -20;

        public static void AdjustWindowRectEx(ref RECT lpRect, WindowStyles dwStyle, WindowStylesEx dwExStyle)
        {
            if (!NativeMethods.AdjustWindowRectEx(ref lpRect, dwStyle, false, dwExStyle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static Window FromHandle(IntPtr hWnd)
        {
            var result = new Window
            {
                Handle = hWnd,
                Class = WindowClass.GetClassOfWindow(hWnd),
            };
            result.Module = result.Class.Module;
            return result;
        }

        public static Window GetWindowIfWindowPatternValid(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return null;

            if (!NativeMethods.IsWindow(hWnd))
                return null;

            if (!NativeMethods.IsWindowVisible(hWnd))
                return null;

            var window = FromHandle(hWnd);

            if (!window.IsWindowPatternValid())
                return null;

            return window;
        }

        public static Window GetDesktopWindow()
        {
            return FromHandle(NativeMethods.GetDesktopWindow());
        }

        #region Window Enumerator

        private class WindowEnumeratorEventArgs : EventArgs
        {
            public IntPtr WindowHandle { get; set; }
        }

        private static EnumWindowsProc _windowEnumerator = new(WindowEnumeratorCallback);

        private static event EventHandler<WindowEnumeratorEventArgs> EnumerateItem;

        private static bool WindowEnumeratorCallback([In] IntPtr hwnd, [In] IntPtr lParam)
        {
            EnumerateItem?.Invoke(null, new WindowEnumeratorEventArgs { WindowHandle = hwnd });
            return true;
        }

        #endregion

        private bool _isDisposing = false;

        public string Title { get; set; }

        public IntPtr Handle { get; protected set; }

        public IntPtr Parent { get; set; } = IntPtr.Zero;

        public IntPtr Module { get; private set; } = NativeMethods.InstanceHandle;

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

        protected virtual IntPtr LoadIcon()
        {
            return LoadIcon(Module, $"#{Macros.IDI_APPLICATION_ICON}");
        }

        protected virtual IntPtr LoadIcon(IntPtr module, string resourceId)
        {
            var result = NativeMethods.LoadIconW(module, resourceId);
            if (result == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return result;
        }

        protected IntPtr GetModule(string moduleName)
        {
            var module = NativeMethods.GetModuleHandleW(moduleName);
            if (module == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                if (error == ERROR_MOD_NOT_FOUND)
                {
                    module = NativeMethods.LoadLibraryW(moduleName);
                    if (module == IntPtr.Zero)
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    throw new Win32Exception(error);
                }
            }
            return module;
        }

        protected virtual void RegisterClass()
        {
            var icon = LoadIcon();
            Class = new WindowClass(Module, $"{GetType().Name}+{Guid.NewGuid()}", WndProc, icon);
        }

        protected virtual void CreateWindow()
        {
            Handle = NativeMethods.CreateWindowExW(
                StyleEx,
                Class.Atom,
                Title,
                Style,
                X, Y, Width, Height,
                Parent,
                IntPtr.Zero,
                Module);

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
            NativeMethods.ShowWindow(Handle, command);
        }

        public virtual void Close()
        {
            if (!NativeMethods.PostMessageW(Handle, (uint)WindowMessage.WM_CLOSE, IntPtr.Zero, IntPtr.Zero))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public virtual void Dispose()
        {
            if (_isDisposing) return;
            _isDisposing = true;
            _ = NativeMethods.DestroyWindow(Handle);
            Handle = IntPtr.Zero;
            Class.Dispose();
            _ = NativeMethods.DestroyIcon(Class.Icon);
        }

        public void UpdateScaleFactor()
        {
            ScaleFactor = ((float)NativeMethods.GetDpiForWindow(Handle)) / USER_DEFAULT_SCREEN_DPI;
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

        public void CenterToWindow(Window parent)
        {
            var parentRect = parent.GetWindowRect();
            X = parentRect.X + (parentRect.Width - Width) / 2;
            Y = parentRect.Y + (parentRect.Height - Height) / 2;
        }

        public void CenterToWindowScaled(Window parent)
        {
            var parentRect = parent.GetWindowRect();
            X = parentRect.X + (int)((parentRect.Width - ScaledWidth) / 2);
            Y = parentRect.Y + (int)((parentRect.Height - ScaledHeight) / 2);
        }

        public string GetText()
        {
            var result = new StringBuilder(Macros.MAX_PATH);
            NativeMethods.GetWindowText(Handle, result, Macros.MAX_PATH);
            return result.ToString();
        }

        public WindowStyles GetWindowStyle()
        {
            var style = NativeMethods.GetWindowLongPtrW(Handle, GWL_STYLE);
            if (style == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new Win32Exception(error);
            }
            return (WindowStyles)unchecked((uint)style.ToInt64());
        }

        public WindowStylesEx GetWindowExStyle()
        {
            var style = NativeMethods.GetWindowLongPtrW(Handle, GWL_EXSTYLE);
            if (style == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new Win32Exception(error);
            }
            return (WindowStylesEx)unchecked((uint)style.ToInt64());
        }

        public void ForEachChild(Action<Window> callback)
        {
            void ForEachChild(object sender, WindowEnumeratorEventArgs args)
            {
                if (args.WindowHandle == IntPtr.Zero)
                    return;

                if (!NativeMethods.IsWindow(args.WindowHandle))
                    return;

                callback?.Invoke(FromHandle(args.WindowHandle));
            }

            EnumerateItem += ForEachChild;
            NativeMethods.EnumChildWindows(Handle, _windowEnumerator, IntPtr.Zero);
            EnumerateItem -= ForEachChild;
        }

        public bool IsWindowPatternValid()
        {
            StyleEx = GetWindowExStyle();
            if (StyleEx.HasFlag(WindowStylesEx.WS_EX_APPWINDOW))
                return true;

            Style = GetWindowStyle();
            if (Style == 0)
                return false;

            var hasTitleBar = Style.HasFlag(WindowStyles.WS_BORDER) &&
                Style.HasFlag(WindowStyles.WS_DLGFRAME);

            if (StyleEx.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW) && !hasTitleBar)
                return false;

            if (Style.HasFlag(WindowStyles.WS_POPUP) && !hasTitleBar)
                return false;

            var desktopWindow = NativeMethods.GetDesktopWindow();
            var parent = NativeMethods.GetAncestor(Handle, GetAncestorFlag.GA_PARENT);
            if (parent != desktopWindow && !StyleEx.HasFlag(WindowStylesEx.WS_EX_MDICHILD))
                return false;

            return true;
        }

        public uint GetProcessId()
        {
            NativeMethods.GetWindowThreadProcessId(Handle, out var pid);
            return pid;
        }

        public bool IsVisible()
        {
            return NativeMethods.IsWindowVisible(Handle);
        }

        public void SetEnable(bool enabled)
        {
            NativeMethods.EnableWindow(Handle, enabled);
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
            return NativeMethods.DefWindowProcW(hwnd, umsg, wParam, lParam);
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
