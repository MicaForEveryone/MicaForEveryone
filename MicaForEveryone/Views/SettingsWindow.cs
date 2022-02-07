using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI;
using MicaForEveryone.UI.Brushes;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.Views
{
    internal class SettingsWindow : XamlWindow
    {
        [DllImport("dwmapi.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DwmDefWindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        private readonly XamlMicaBrush _backgroundBrush;

        public SettingsWindow() : this(new())
        {
        }

        private SettingsWindow(SettingsView view) : base(view)
        {
            Style = WindowStyles.WS_OVERLAPPEDWINDOW & ~WindowStyles.WS_MAXIMIZEBOX & ~WindowStyles.WS_MINIMIZEBOX;
            Width = 820;
            Height = 560;

            _backgroundBrush = new XamlMicaBrush(View, this);

            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("SettingsTitle/Text");

            view.ViewModel = ViewModel;
            view.ActualThemeChanged += View_ActualThemeChanged;
            view.Loaded += View_Loaded;
        }

        private ISettingsViewModel ViewModel { get; } =
            Program.CurrentApp.Container.GetService<ISettingsViewModel>();

        public override void Activate()
        {
            base.Activate();

            CenterToWindowScaled(GetDesktopWindow());
            UpdatePosition();

            ((Grid)((SettingsView)View).Content).Background = _backgroundBrush;

            ShowWindow();
            SetForegroundWindow();
        }

        protected override void UpdateXamlSourcePosition()
        {
            if (Interop == null) return;
            var clientArea = GetClientRect();
            var xborder = (int)(SystemMetrics.FrameWidth * ScaleFactor);
            var yborder = (int)(SystemMetrics.FrameHeight * ScaleFactor);
            var captionHeight = (int)(SystemMetrics.CaptionHeight * ScaleFactor);
            var xamlWindow = FromHandle(Interop.WindowHandle);
            xamlWindow.X = clientArea.left + xborder;
            xamlWindow.Y = clientArea.top + captionHeight;
            xamlWindow.Width = clientArea.right - 2*xborder;
            xamlWindow.Height = clientArea.bottom - yborder - captionHeight;
            xamlWindow.SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        // WndProc and HitTestNCA, based on codes from https://docs.microsoft.com/en-us/windows/win32/dwm/customframe

        protected override IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            var fCallDWP = !DwmDefWindowProc(hwnd, umsg, wParam, lParam, out var plResult);

            if (umsg == (uint)WindowMessage.WM_CREATE)
            {
                DesktopWindowManager.ExtendFrameIntoClientArea(hwnd);
                DesktopWindowManager.SetImmersiveDarkMode(hwnd, Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode == TitlebarColorMode.Dark);
                DesktopWindowManager.EnableMicaIfSupported(hwnd);

                Handle = hwnd;
                var clientArea = GetClientRect();
                X = clientArea.left;
                Y = clientArea.top;
                Width = clientArea.Width;
                Height = clientArea.Height;
                SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_FRAMECHANGED);

                fCallDWP = true;
            }
            else if (umsg == (uint)WindowMessage.WM_NCCALCSIZE && wParam != IntPtr.Zero)
            {
                // remove standard frame
                plResult = IntPtr.Zero;
                fCallDWP = false;
            }
            else if (umsg == (uint)WindowMessage.WM_NCHITTEST && plResult == IntPtr.Zero)
            {
                // hit test non-client area
                var result = HitTestNCA(lParam);
                plResult = (IntPtr)result;

                if (result != HitTestValues.HTNOWHERE)
                {
                    fCallDWP = false;
                }
            }
            else if (umsg == (uint)WindowMessage.WM_SETTINGCHANGE)
            {
                SystemMetrics.Refresh();
            }

            return fCallDWP ? base.WndProc(hwnd, umsg, wParam, lParam) : plResult;
        }

        // Hit test the frame for resizing and moving.
        private HitTestValues HitTestNCA(IntPtr lParam)
        {
            var xborder = (int)(SystemMetrics.FrameWidth * ScaleFactor);
            var yborder = (int)(SystemMetrics.FrameHeight * ScaleFactor);
            var captionHeight = (int)(SystemMetrics.CaptionHeight * ScaleFactor);

            // Get the point coordinates for the hit test.
            var ptMouseX = Macros.GET_X_LPARAM(lParam);
            var ptMouseY = Macros.GET_Y_LPARAM(lParam);

            // Get the window rectangle.
            var rcWindow = GetWindowRect();

            // Get the frame rectangle, adjusted for the style without a caption.
            var rcFrame = new RECT();
            AdjustWindowRectEx(ref rcFrame, WindowStyles.WS_OVERLAPPEDWINDOW & ~WindowStyles.WS_CAPTION, 0);

            // Determine if the hit test is for resizing. Default middle (1,1).
            byte uRow = 1;
            byte uCol = 1;
            bool fOnResizeBorder = false;

            // Determine if the point is at the top or bottom of the window.
            if (ptMouseY >= rcWindow.top && ptMouseY < rcWindow.top + captionHeight)
            {
                fOnResizeBorder = (ptMouseY < (rcWindow.top - rcFrame.top));
                uRow = 0;
            }
            else if (ptMouseY < rcWindow.bottom && ptMouseY >= rcWindow.bottom - yborder)
            {
                uRow = 2;
            }

            // Determine if the point is at the left or right of the window.
            if (ptMouseX >= rcWindow.left && ptMouseX < rcWindow.left + xborder)
            {
                uCol = 0; // left side
            }
            else if (ptMouseX < rcWindow.right && ptMouseX >= rcWindow.right - xborder)
            {
                uCol = 2; // right side
            }

            // Hit test (HTTOPLEFT, ... HTBOTTOMRIGHT)
            var htCaptionOrHtTop = fOnResizeBorder ? HitTestValues.HTTOP : HitTestValues.HTCAPTION;
            var hitTests = new[]
            {
                new[] { HitTestValues.HTTOPLEFT,    htCaptionOrHtTop,        HitTestValues.HTTOPRIGHT },
                new[] { HitTestValues.HTLEFT,       HitTestValues.HTNOWHERE, HitTestValues.HTRIGHT },
                new[] { HitTestValues.HTBOTTOMLEFT, HitTestValues.HTBOTTOM,  HitTestValues.HTBOTTOMRIGHT },
            };

            return hitTests[uRow][uCol];
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(sender);
        }

        private void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            DesktopWindowManager.SetImmersiveDarkMode(Handle, Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode == TitlebarColorMode.Dark);
        }
    }
}
