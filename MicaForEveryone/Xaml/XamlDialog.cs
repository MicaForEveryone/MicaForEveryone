using System;
using Microsoft.Toolkit.Win32.UI.XamlHost;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Xaml
{
    public class XamlDialog : Dialog
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();
        
        public XamlDialog(FrameworkElement view)
        {
            _xamlSource.Content = view;
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

            SizeChanged += XamlDialog_SizeChanged;
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
            xamlWindow.SetWindowPosScaled(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlDialog_SizeChanged(object sender, WndProcEventArgs e)
        {
            UpdateXamlSourcePosition();
        }
    }
}
