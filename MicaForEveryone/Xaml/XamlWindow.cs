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
            View = view;
            Create += XamlWindow_Create;
        }

        public UIElement View { get; protected set; }

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() => 
            _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();

        private void XamlWindow_Create(object sender, Win32.WindowEventArgs e)
        {
            _xamlSource = new() { Content = View };
            var interop = GetXamlWindowInterop();
            interop.AttachToWindow(e.WindowHandle);
            
            GetWindowRect(e.WindowHandle, out var clientArea);
            interop.WindowHandle.SetWindowPos(HWND.NULL, clientArea, 
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }
    }
}
