using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Vanara.PInvoke;

using MicaForEveryone.Extensions;
using MicaForEveryone.Win32;
using MicaForEveryone.UI;
using MicaForEveryone.Xaml;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Views
{
    public class MainWindow : XamlWindow
    {
        private const uint WM_APP_NOTIFYICON = WM_APP + 1;

        public const uint WM_APP_REMATCH_REQUEST = WM_APP + 2;
        public const uint WM_APP_RELOAD_CONFIG = WM_APP + 3;

        private NotifyIcon _notifyIcon;

        public MainWindow() : base(new TrayIconView())
        {
            ClassName = nameof(MainWindow);
            Title = "Mica For Everyone";
            Icon = LoadIcon(HINSTANCE.NULL, IDI_APPLICATION);
            Style = WindowStyles.WS_POPUPWINDOW;
            StyleEx = WindowStylesEx.WS_EX_TOPMOST;

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

        public override void Activate()
        {
            base.Activate();
            _notifyIcon.Parent = Handle;
            _notifyIcon.Activate();
            _notifyIcon.Show();
        }

        public void RequestRematchRules()
        {
            PostMessage(Handle, WM_APP_REMATCH_REQUEST);
        }

        public void RequestReloadConfig()
        {
            PostMessage(Handle, WM_APP_RELOAD_CONFIG);
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

                Interop.WindowHandle.SetWindowPos(
                    HWND.NULL,
                    new RECT(0, 0, notifyIconRect.Width, notifyIconRect.Height),
                    SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                menu.ShowAt(View,
                    new Point(
                        (x - notifyIconRect.X) / ScaleFactor,
                        (y - notifyIconRect.Y) / ScaleFactor));
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

            Interop.WindowHandle.SetWindowPos(
                HWND.NULL,
                new RECT(0, 0, notifyIconRect.Width, notifyIconRect.Height),
                SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            ((ToolTip)ToolTipService.GetToolTip(View)).IsOpen = true;
        }

        private void NotifyIcon_ClosePopup(object sender, EventArgs e)
        {
            ((ToolTip)ToolTipService.GetToolTip(View)).IsOpen = false;
        }

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch (umsg)
            {
                case WM_APP_REMATCH_REQUEST:
                    RematchRulesRequested?.Invoke(this, EventArgs.Empty);
                    break;

                case WM_APP_RELOAD_CONFIG:
                    ReloadConfigRequested?.Invoke(this, EventArgs.Empty);
                    break;
            }

            return base.WndProc(hwnd, umsg, wParam, lParam);
        }

        public event EventHandler RematchRulesRequested;
        public event EventHandler ReloadConfigRequested;
    }
}
