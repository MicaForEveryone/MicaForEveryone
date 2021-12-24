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
        }

        public FrameworkElement View => (FrameworkElement)_xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 Interop { get; private set; }

        public Windows.UI.Core.CoreDispatcher Dispatcher { get; private set; }

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
            Dispatcher = Window.Current.Dispatcher;
        }

        public void UpdateXamlSourcePosition()
        {
            GetClientRect(Handle, out var clientArea);
            Interop?.WindowHandle.SetWindowPos(HWND.NULL, clientArea, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlDialog_SizeChanged(object sender, Win32.WindowEventArgs e)
        {
            UpdateXamlSourcePosition();
        }
    }
}
