using System;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Xaml
{
    public class XamlDialog : Win32.Dialog
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();

        public XamlDialog(FrameworkElement view)
        {
            _xamlSource.Content = view;
            Create += XamlDialog_Create;
        }

        public FrameworkElement View => (FrameworkElement)_xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() =>
            _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();

        public override void Dispose()
        {
            _xamlSource.Dispose();
            base.Dispose();
        }

        private void XamlDialog_Create(object sender, Win32.WindowEventArgs args)
        {
            var interop = GetXamlWindowInterop();
            interop.AttachToWindow(args.WindowHandle);

            GetClientRect(args.WindowHandle, out var clientArea);
            interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, clientArea.Width, clientArea.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }
    }
}
