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
        private DesktopWindowXamlSource _xamlSource;

        public XamlDialog(UIElement view)
        {
            _xamlSource = new() { Content = view };
            Style = WindowStyles.WS_DLGFRAME;
        }

        public UIElement View => _xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() => 
            _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();

        public override void Activate()
        {
            base.Activate();

            var interop = GetXamlWindowInterop();
            interop.AttachToWindow(Handle);
            
            GetClientRect(Handle, out var clientArea);
            interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, clientArea.Width, clientArea.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }
    }
}
