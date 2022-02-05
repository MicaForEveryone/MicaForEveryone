using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI;
using MicaForEveryone.UI.Brushes;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.Views
{
    internal class SettingsWindow : XamlWindow
    {
        private readonly XamlMicaBrush _backgroundBrush;

        private int _captionHeight = GetSystemMetrics(SystemMetric.SM_CYCAPTION);
        private int _frameWidth = GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);
        private int _frameHeight = GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);

        public SettingsWindow() : this(new())
        {
        }

        private SettingsWindow(SettingsView view) : base(view)
        {
            ClassName = nameof(SettingsWindow);
            Style = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE;
            Width = 800;
            Height = 540;
            Icon = LoadIcon(Kernel32.GetModuleHandle(null), IDI_APPLICATION);

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

            ((Grid)((SettingsView)View).Content).Background = _backgroundBrush;

            SetForegroundWindow();
        }

        protected override void UpdateXamlSourcePosition()
        {
            GetClientRect(Handle, out var clientArea);
            var xborder = (int)(_frameWidth * ScaleFactor);
            var yborder = (int)(_frameHeight * ScaleFactor);
            var captionHeight = (int)(_captionHeight * ScaleFactor);
            clientArea.left += xborder;
            clientArea.right -= xborder;
            clientArea.top += captionHeight;
            clientArea.bottom -= yborder;
            Interop?.WindowHandle.SetWindowPos(HWND.NULL, clientArea, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_SHOWWINDOW);
        }

        // WndProc and HitTestNCA, based on codes from https://docs.microsoft.com/en-us/windows/win32/dwm/customframe

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            var fCallDWP = !DwmApi.DwmDefWindowProc(hwnd, umsg, wParam, lParam, out var plResult);

            if (umsg == (uint)WindowMessage.WM_CREATE)
            {
                hwnd.ExtendFrameIntoClientArea();
                hwnd.ApplyBackdropRule(BackdropType.Mica);
                hwnd.ApplyTitlebarColorRule(
                    Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode,
                    TitlebarColorMode.Default);

                GetWindowRect(hwnd, out var lpRect);
                SetWindowPos(hwnd,
                    HWND.NULL,
                    lpRect.left, lpRect.top, lpRect.Width, lpRect.Height,
                    SetWindowPosFlags.SWP_FRAMECHANGED);

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
                var result = HitTestNCA(hwnd, wParam, lParam);
                plResult = (IntPtr)result;

                if (result != HitTestValues.HTNOWHERE)
                {
                    fCallDWP = false;
                }
            }
            else if (umsg == (uint)WindowMessage.WM_SETTINGCHANGE)
            {
                // update frame size when system settings changed
                _captionHeight = GetSystemMetrics(SystemMetric.SM_CYCAPTION);
                _frameWidth = GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);
                _frameHeight = GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);
            }

            return fCallDWP ? base.WndProc(hwnd, umsg, wParam, lParam) : plResult;
        }

        // Hit test the frame for resizing and moving.
        private HitTestValues HitTestNCA(HWND hWnd, IntPtr wParam, IntPtr lParam)
        {
            var xborder = (int)(_frameWidth * ScaleFactor);
            var yborder = (int)(_frameHeight * ScaleFactor);
            var captionHeight = (int)(_captionHeight * ScaleFactor);

            // Get the point coordinates for the hit test.
            var ptMouseX = Macros.GET_X_LPARAM(lParam);
            var ptMouseY = Macros.GET_Y_LPARAM(lParam);

            // Get the window rectangle.
            GetWindowRect(hWnd, out var rcWindow);

            // Get the frame rectangle, adjusted for the style without a caption.
            var rcFrame = new RECT();
            AdjustWindowRectEx(ref rcFrame,
                WindowStyles.WS_OVERLAPPEDWINDOW & ~WindowStyles.WS_CAPTION,
                false,
                0);

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
            var htCaptionOrHtTop = fOnResizeBorder? HitTestValues.HTTOP : HitTestValues.HTCAPTION;
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
            Handle.ApplyTitlebarColorRule(
                Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode,
                TitlebarColorMode.Default);
        }
    }
}
