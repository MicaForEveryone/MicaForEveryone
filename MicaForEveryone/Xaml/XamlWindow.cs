using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Xaml
{
    public class XamlWindow : Win32.Window
    {
        private DesktopWindowXamlSource _xamlSource;

        public XamlWindow(UIElement view)
        {
            View = view;
            Activated += XamlWindow_Activated;
        }

        public UIElement View { get; protected set; }

        private void XamlWindow_Activated(object sender, EventArgs args)
        {
            _xamlSource = new()
            {
                Content = View,
            };
            var interop = GetXamlWindowInterop();
            interop.AttachToWindow(Handle);
            interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, Size.Width, Size.Height),
                User32.SetWindowPosFlags.SWP_NOZORDER | User32.SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() => _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();
    }
}
