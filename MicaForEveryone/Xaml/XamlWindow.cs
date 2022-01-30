using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Core;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Win32;
using MicaForEveryone.Models;
using System;

namespace MicaForEveryone.Xaml
{
    public class XamlWindow : NativeWindow, IFocusableWindow
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();

        public XamlWindow(FrameworkElement view)
        {
            _xamlSource.Content = view;
            SizeChanged += XamlWindow_SizeChanged;
        }

        public FrameworkElement View => (FrameworkElement)_xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 Interop { get; private set; }

        public CoreDispatcher Dispatcher { get; private set; }

        public override void Dispose()
        {
            _xamlSource.Dispose();
            base.Dispose();
        }

        public override void Activate()
        {
            base.Activate();
            Interop = _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();
            Interop.AttachToWindow(Handle);
            UpdateXamlSourcePosition();
            Dispatcher = View.Dispatcher;
        }

        protected virtual void UpdateXamlSourcePosition()
        {
            GetClientRect(Handle, out var clientArea);
            Interop?.WindowHandle.SetWindowPos(HWND.NULL, clientArea, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlWindow_SizeChanged(object sender, Win32EventArgs e)
        {
            UpdateXamlSourcePosition();
        }

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            if (umsg == (uint)WindowMessage.WM_ACTIVATE)
            {
                if (Macros.LOWORD(wParam) == 0) // WA_INACTIVE = 0
                {
                    LostFocus?.Invoke(this, EventArgs.Empty);
                }
                else // WA_ACTIVE or WA_CLICKACTIVE
                {
                    GotFocus?.Invoke(this, EventArgs.Empty);
                }
            }
            return base.WndProc(hwnd, umsg, wParam, lParam);
        }

        public event EventHandler GotFocus;
        public event EventHandler LostFocus;
    }
}
