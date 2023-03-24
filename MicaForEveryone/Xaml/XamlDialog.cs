using System;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Win32.UI.XamlHost;

using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;
using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Xaml
{
    public class XamlDialog : Dialog
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();

        public XamlDialog(FrameworkElement view)
        {
            _xamlSource.Content = view;
            Interop = _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();
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
            var svcLanguage = Program.Container.GetService<ILanguageService>();
            if (svcLanguage?.CurrentLanguage.LayoutDirection is LanguageLayoutDirection.Rtl
                or LanguageLayoutDirection.TtbRtl)
            {
                View.FlowDirection = FlowDirection.RightToLeft;
                StyleEx |= WindowStylesEx.WS_EX_LAYOUTRTL;
            }

            base.Activate();

            Interop.AttachToWindow(Handle);

            UpdateXamlSourcePosition();
            SizeChanged += XamlDialog_SizeChanged;

            // set focus to xaml content on activation
            _ = _xamlSource.NavigateFocus(
                new XamlSourceFocusNavigationRequest(
                    XamlSourceFocusNavigationReason.Programmatic));
        }

        protected virtual void UpdateXamlSourcePosition()
        {
            var clientArea = GetClientRect();
            var xamlWindow = FromHandle(Interop.WindowHandle);
            xamlWindow.X = clientArea.X;
            xamlWindow.Y = clientArea.Y;
            xamlWindow.Width = clientArea.Width;
            xamlWindow.Height = clientArea.Height;
            xamlWindow.SetWindowPosScaled(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlDialog_SizeChanged(object? sender, WndProcEventArgs e)
        {
            UpdateXamlSourcePosition();
        }
    }
}
