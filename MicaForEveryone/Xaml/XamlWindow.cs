using System;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Win32.UI.XamlHost;

using MicaForEveryone.Core.Ui.Interfaces;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

#nullable enable

namespace MicaForEveryone.Xaml
{
    public class XamlWindow : Win32.Window, IFocusableWindow
    {
        private readonly DesktopWindowXamlSource _xamlSource = new();

        public XamlWindow(FrameworkElement view)
        {
            _xamlSource.Content = view;
            Interop = _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();
        }

        public FrameworkElement View => (FrameworkElement)_xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 Interop { get; }

        public override void Dispose()
        {
            _xamlSource.Dispose();
            base.Dispose();
        }

        public override void Activate()
        {
            // set direction to rtl if a rtl language is chosen
            var svcLanguage = Program.Container.GetService<ILanguageService>();
            if (svcLanguage?.CurrentLanguage.LayoutDirection is LanguageLayoutDirection.Rtl
                or LanguageLayoutDirection.TtbRtl)
            {
                View.FlowDirection = FlowDirection.RightToLeft;
                StyleEx |= WindowStylesEx.WS_EX_LAYOUTRTL;
            }

            base.Activate();

            // attach xaml islands to win32 window
            Interop.AttachToWindow(Handle);

            // update position of xaml content in the win32 window
            UpdateXamlSourcePosition();

            // update xaml content position when win32 window size is changed
            SizeChanged += XamlWindow_SizeChanged;

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
            xamlWindow.SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        private void XamlWindow_SizeChanged(object? sender, WndProcEventArgs e)
        {
            UpdateXamlSourcePosition();
        }

        protected override IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            // notify when window got or lost focus
            if (umsg == (uint)WindowMessage.WM_ACTIVATE)
            {
                if (Macros.LOWORD(wParam) == 0) // WA_INACTIVE = 0
                {
                    LostFocus?.Invoke(this, EventArgs.Empty);
                }
                else // WA_ACTIVE or WA_CLICKACTIVE
                {
                    GotFocus?.Invoke(this, EventArgs.Empty);
                }
            }
            return base.WndProc(hwnd, umsg, wParam, lParam);
        }

        public event EventHandler? GotFocus;
        public event EventHandler? LostFocus;
    }
}
