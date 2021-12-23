using System;
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
        private readonly DesktopWindowXamlSource _xamlSource = new();

        public XamlWindow(FrameworkElement view)
        {
            _xamlSource.Content = view;
            Create += XamlWindow_Create;
        }

        public FrameworkElement View => (FrameworkElement)_xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() =>
            _xamlSource?.GetInterop<IDesktopWindowXamlSourceNative2>();

        public override void Dispose()
        {
            _xamlSource.Dispose();
            base.Dispose();
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
    }
}
