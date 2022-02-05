using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Core;
using Microsoft.Toolkit.Win32.UI.XamlHost;

using MicaForEveryone.Models;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Xaml
{
    public class XamlWindow : Win32.Window, IFocusableWindow
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();

        public XamlWindow(FrameworkElement view)
        {
            _xamlSource.Content = view;
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

            SizeChanged += XamlWindow_SizeChanged;
        }

        protected virtual void UpdateXamlSourcePosition()
        {
            if (Interop == null) return;
            var clientArea = GetClientRect();
            var xamlWindow = FromHandle(Interop.WindowHandle);
            xamlWindow.X = clientArea.X;
            xamlWindow.Y = clientArea.Y;
            xamlWindow.Width = clientArea.Width;
            xamlWindow.Height = clientArea.Height;
            xamlWindow.SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlWindow_SizeChanged(object sender, WndProcEventArgs e)
        {
            UpdateXamlSourcePosition();
        }

        protected override IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
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
