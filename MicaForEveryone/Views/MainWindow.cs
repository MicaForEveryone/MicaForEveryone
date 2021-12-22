using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Vanara.PInvoke;

using MicaForEveryone.Extensions;
using MicaForEveryone.Win32;
using MicaForEveryone.UWP;
using MicaForEveryone.Xaml;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Views
{
    public class MainWindow : XamlWindow
    {
        private const uint WM_APP_NOTIFYICON = WM_APP + 1;
        private const uint USER_DEFAULT_SCREEN_DPI = 96;

        private NotifyIcon _notifyIcon;

        public MainWindow() : base(new MainWindowView())
        {
            ClassName = nameof(MainWindow);
            Title = "Mica For Everyone";
            Icon = LoadIcon(HINSTANCE.NULL, IDI_APPLICATION);
            Style = WindowStyles.WS_POPUPWINDOW;
            StyleEx = WindowStylesEx.WS_EX_TOPMOST;

            Create += MainWindow_Create;
            Destroy += MainWindow_Destroy;

            _notifyIcon = new NotifyIcon
            {
                CallbackMessage = WM_APP_NOTIFYICON,
                Id = 0,
                Title = "Mica For Everyone",
                Icon = Icon,
            };

            _notifyIcon.Click += NotifyIcon_ContextMenu;
            _notifyIcon.ContextMenu += NotifyIcon_ContextMenu;
            _notifyIcon.OpenPopup += NotifyIcon_OpenPopup;
            _notifyIcon.ClosePopup += NotifyIcon_ClosePopup;
        }

        private void MainWindow_Create(object sender, WindowEventArgs e)
        {
            _notifyIcon.Parent = e.WindowHandle;
            _notifyIcon.Activate();
            _notifyIcon.Show();
        }

        private void MainWindow_Destroy(object sender, WindowEventArgs e)
        {
            _notifyIcon.Hide();
        }

        private void ShowContextFlyout(int x, int y)
        {
            if (View.ContextFlyout is MenuFlyout menu)
            {
                if (menu.IsOpen)
                {
                    menu.Hide();
                    return;
                }

                SetForegroundWindow(Handle);

                var notifyIconRect = _notifyIcon.GetRect();

                Handle.SetWindowPos(
                    HWND.NULL,
                    notifyIconRect,
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                GetXamlWindowInterop().WindowHandle.SetWindowPos(
                    HWND.NULL,
                    new RECT(0, 0, notifyIconRect.Width, notifyIconRect.Height),
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                var scaleFactor = ((float)GetDpiForWindow(Handle)) / USER_DEFAULT_SCREEN_DPI;

                menu.ShowAt(View, 
                    new Point(
                        (x - notifyIconRect.X)/scaleFactor,
                        (y - notifyIconRect.Y)/scaleFactor));
            }
        }

        private void NotifyIcon_ContextMenu(object sender, TrayIconClickEventArgs e)
        {
            ShowContextFlyout(e.Point.X, e.Point.Y);
        }

        private void NotifyIcon_OpenPopup(object sender, EventArgs e)
        {
            var notifyIconRect = _notifyIcon.GetRect();

            Handle.SetWindowPos(
                    HWND.NULL,
                    notifyIconRect,
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            GetXamlWindowInterop().WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, notifyIconRect.Width, notifyIconRect.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            ((ToolTip)ToolTipService.GetToolTip(View)).IsOpen = true;
        }

        private void NotifyIcon_ClosePopup(object sender, EventArgs e)
        {
            ((ToolTip)ToolTipService.GetToolTip(View)).IsOpen = false;
        }
    }
}
