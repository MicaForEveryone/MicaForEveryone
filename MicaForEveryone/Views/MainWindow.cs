using System;
using Microsoft.Extensions.DependencyInjection;
using Vanara.PInvoke;

using MicaForEveryone.Win32;
using MicaForEveryone.UI;
using MicaForEveryone.Xaml;
using MicaForEveryone.ViewModels;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Views
{
    internal class MainWindow : XamlWindow
    {
        private const uint WM_APP_NOTIFYICON = WM_APP + 1;

        public const uint WM_APP_REMATCH_REQUEST = WM_APP + 2;
        public const uint WM_APP_RELOAD_CONFIG = WM_APP + 3;
        public const uint WM_APP_SAVE_CONFIG_REQUESTED = WM_APP + 4;

        private NotifyIcon _notifyIcon;

        public MainWindow() : this(new())
        {
        }

        private MainWindow(TrayIconView view) : base(view)
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

            view.ViewModel = ViewModel;
            view.Loaded += View_Loaded;
            view.ActualThemeChanged += View_ActualThemeChanged;
        }

        public ITrayIconViewModel ViewModel { get; } = 
            Program.CurrentApp.Container.GetService<ITrayIconViewModel>();

        public override void Activate()
        {
            base.Activate();
            _notifyIcon.Parent = Handle;
            _notifyIcon.Activate();
            _notifyIcon.Show();
        }

        public override void Dispose()
        {
            _notifyIcon.Dispose();
            base.Dispose();
        }

        public void RequestRematchRules()
        {
            PostMessage(Handle, WM_APP_REMATCH_REQUEST);
        }

        public void RequestReloadConfig()
        {
            PostMessage(Handle, WM_APP_RELOAD_CONFIG);
        }

        public void RequestSaveConfig()
        {
            PostMessage(Handle, WM_APP_SAVE_CONFIG_REQUESTED);
        }

        // event handlers
        private void MainWindow_Destroy(object sender, Win32EventArgs e)
        {
            _notifyIcon.Hide();
        }

        private void NotifyIcon_ContextMenu(object sender, TrayIconClickEventArgs e)
        {
            var notifyIconRect = _notifyIcon.GetRect();
            ViewModel.ShowContextMenu(e.Point, notifyIconRect);
        }

        private void NotifyIcon_OpenPopup(object sender, EventArgs e)
        {
            var notifyIconRect = _notifyIcon.GetRect();
            ViewModel.ShowTipPopup(notifyIconRect);
        }

        private void NotifyIcon_ClosePopup(object sender, EventArgs e)
        {
            ViewModel.HideTipPopup();
        }

        private void View_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.InitializeApp(this);
        }

        private void View_ActualThemeChanged(Windows.UI.Xaml.FrameworkElement sender, object args)
        {
            RequestRematchRules();
        }

        protected override IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch (umsg)
            {
                case WM_APP_REMATCH_REQUEST:
                    ViewModel.RematchRules();
                    break;

                case WM_APP_RELOAD_CONFIG:
                    ViewModel.ReloadConfig();
                    break;

                case WM_APP_SAVE_CONFIG_REQUESTED:
                    ViewModel.SaveConfig();
                    break;
            }

            return base.WndProc(hwnd, umsg, wParam, lParam);
        }
    }
}
