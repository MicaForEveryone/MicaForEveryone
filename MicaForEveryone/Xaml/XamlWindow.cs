using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Xaml
{
    public class XamlWindow : Win32.Window
    {
        private DesktopWindowXamlSource _xamlSource;

        public XamlWindow(UIElement view)
        {
            _xamlSource = new() { Content = view };
            Create += XamlWindow_Create;
        }

        public UIElement View => _xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() =>
            _xamlSource?.GetInterop<IDesktopWindowXamlSourceNative2>();

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch ((WindowMessage)umsg)
            {
                case WindowMessage.WM_SETTINGCHANGE:
                case WindowMessage.WM_THEMECHANGED:
                    ThemeChanged?.Invoke(this, EventArgs.Empty);
                    break;
            }
            return base.WndProc(hwnd, umsg, wParam, lParam);
        }

        private void XamlWindow_Create(object sender, Win32.WindowEventArgs e)
        {
            var interop = GetXamlWindowInterop();
            interop.AttachToWindow(e.WindowHandle);

            GetClientRect(e.WindowHandle, out var clientArea);
            interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, clientArea.Width, clientArea.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        public event EventHandler ThemeChanged;
    }
}
