using Microsoft.Toolkit.Win32.UI.XamlHost;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Win32;

namespace MicaForEveryone.Xaml
{
    public class XamlDialog : Win32.Dialog
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();
        
        public XamlDialog(FrameworkElement view)
        {
            _xamlSource.Content = view;
            SizeChanged += XamlDialog_SizeChanged;
        }

        public FrameworkElement View => (FrameworkElement)_xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 Interop { get; private set; }

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
        }

        public void UpdateXamlSourcePosition()
        {
            GetClientRect(Handle, out var clientArea);
            Interop?.WindowHandle.SetWindowPos(HWND.NULL, clientArea, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlDialog_SizeChanged(object sender, Win32EventArgs e)
        {
            UpdateXamlSourcePosition();
        }
    }
}
