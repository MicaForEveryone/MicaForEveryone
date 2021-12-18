using System;
using Windows.UI.Xaml.Controls;
using Vanara.PInvoke;

using MicaForEveryone.Win32;
using MicaForEveryone.UWP;
using MicaForEveryone.Xaml;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Views
{
    public class MainWindow : XamlWindow
    {
        private const uint WM_APP_NOTIFYICON = WM_APP + 1;

        private NotifyIcon _notifyIcon;

        public MainWindow() : base(new MainWindowView())
        {
            ClassName = nameof(MainWindow);
            Title = "Mica For Everyone";
            Icon = LoadIcon(HINSTANCE.NULL, IDI_APPLICATION);
            Parent = HWND.HWND_MESSAGE;

            Activated += MainWindow_Activated;
            Destroy += MainWindow_Destroy;

            _notifyIcon = new NotifyIcon
            {
                CallbackId = WM_APP_NOTIFYICON,
                Id = 0,
                Title = "Mica For Everyone",
                Icon = Icon,
            };

            _notifyIcon.ContextMenu += NotifyIcon_ContextMenu;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            _notifyIcon.Parent = Handle;
            _notifyIcon.Activate();
        }

        private void MainWindow_Destroy(object sender, EventArgs e)
        {
            _notifyIcon.Deactivate();
        }

        private void NotifyIcon_ContextMenu(object sender, EventArgs e)
        {
            if (View.ContextFlyout is MenuFlyout menu)
            {
                var position = _notifyIcon.GetRect();
                Move(position, false);
                menu.ShowAt(View, new Windows.Foundation.Point(position.X, position.Y));
            }
        }
    }
}
