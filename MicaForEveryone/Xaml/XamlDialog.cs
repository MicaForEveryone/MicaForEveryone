﻿using Microsoft.Toolkit.Win32.UI.XamlHost;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Extensions;
using System;
using Windows.UI.ViewManagement;
using Windows.UI.Core;

namespace MicaForEveryone.Xaml
{
    public class XamlDialog : Win32.Dialog
    {
        private DesktopWindowXamlSource _xamlSource;

        public XamlDialog(UIElement view)
        {
            _xamlSource = new() { Content = view };
        }

        public UIElement View => _xamlSource.Content;

        public IDesktopWindowXamlSourceNative2 GetXamlWindowInterop() =>
            _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();

        public override void Activate()
        {
            base.Activate();

            Handle.ExtendFrameIntoClientArea();

            Handle.SetWindowPos(
                HWND.NULL,
                RECT.Empty,
                SetWindowPosFlags.SWP_FRAMECHANGED |
                SetWindowPosFlags.SWP_NOSIZE |
                SetWindowPosFlags.SWP_NOMOVE);

            var interop = GetXamlWindowInterop();
            interop.AttachToWindow(Handle);

            GetClientRect(Handle, out var clientArea);
            interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, clientArea.Width, clientArea.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            if (umsg == (uint)WindowMessage.WM_NCCALCSIZE && wParam != IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            if (umsg == (uint)WindowMessage.WM_NCHITTEST)
            {
                return IntPtr.Zero;
            }

            return base.WndProc(hwnd, umsg, wParam, lParam);
        }
    }
}