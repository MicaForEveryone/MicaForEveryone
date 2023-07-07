using System;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using MicaForEveryone.Core.Ui.Views;
using MicaForEveryone.Core.Ui.ViewModels;
using MicaForEveryone.UI;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;
using MicaForEveryone.Xaml;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Core.Interfaces;

namespace MicaForEveryone.Views
{
    internal class MainWindow : XamlWindow, ITrayIconView
    {
        public const string OpenSettingsMessage = "MicaForEveryone_OpenSettings";

        private const uint WM_APP_NOTIFYICON = Macros.WM_APP + 1;

        private readonly NotifyIcon _notifyIcon;
        private uint _openSettingsMessage;

        public MainWindow() : this(new())
        {
        }

        private MainWindow(TrayIconView view) : base(view)
        {
            Style = WindowStyles.WS_POPUPWINDOW;
            StyleEx = WindowStylesEx.WS_EX_TOPMOST;

            Destroy += MainWindow_Destroy;

            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("AppName");

            _notifyIcon = new NotifyIcon
            {
                CallbackMessage = WM_APP_NOTIFYICON,
                Id = 0,
                Title = Title,
            };

            _notifyIcon.Click += NotifyIcon_Click;
            _notifyIcon.ContextMenu += NotifyIcon_ContextMenu;
            _notifyIcon.OpenPopup += NotifyIcon_OpenPopup;
            _notifyIcon.ClosePopup += NotifyIcon_ClosePopup;

            view.ViewModel = ViewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ITrayIconViewModel.TrayIconVisible))
            {
                if (ViewModel.TrayIconVisible)
                {
                    _notifyIcon.ShowNotifyIcon();
                }
                else
                {
                    _notifyIcon.HideNotifyIcon();
                }
            }
        }

        public ITrayIconViewModel ViewModel { get; } =
            Program.Container.GetService<ITrayIconViewModel>();

        public override void Activate()
        {
            base.Activate();

            _openSettingsMessage = RegisterWindowMessage(OpenSettingsMessage);

            _notifyIcon.Parent = Handle;
            _notifyIcon.Activate();

            if (ViewModel.TrayIconVisible)
            {
                _notifyIcon.ShowNotifyIcon();
            }

            try
            {
                ViewModel.Attach(this);
            }
#if DEBUG
            catch
            {
                throw;
            }
#else
            catch (Exception ex)
            {
                Program.Container.GetRequiredService<IViewService>().DispatcherEnqueue(() =>
                {
                    var title = ResourceLoader.GetForCurrentView().GetString("AppInitializationError/Title");
                    var dialogService = Program.Container.GetService<IDialogService>();
                    dialogService.ShowErrorDialog(null, title, ex, 500, 320).Destroy += (sender, args) =>
                    {
                        Program.Container.GetRequiredService<IAppLifeTimeService>().ShutdownViewService();
                    };
                });
            }
#endif
        }

        public override void Dispose()
        {
            _notifyIcon.Dispose();
            base.Dispose();
        }

        protected override IntPtr WndProc(IntPtr hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            if (umsg == _openSettingsMessage)
            {
                ViewModel.OpenSettingsCommand.Execute(null);
                return IntPtr.Zero;
            }
            return base.WndProc(hwnd, umsg, wParam, lParam);
        }

        // event handlers

        private void MainWindow_Destroy(object sender, WndProcEventArgs e)
        {
            _notifyIcon.HideNotifyIcon();
        }

        private void NotifyIcon_Click(object sender, TrayIconClickEventArgs e)
        {
            ViewModel.OpenSettingsCommand.Execute(null);
        }

        private void NotifyIcon_ContextMenu(object sender, TrayIconClickEventArgs e)
        {
            var notifyIconRect = _notifyIcon.GetRect();
            ShowContextMenu(e.Point, notifyIconRect);
        }

        private void NotifyIcon_OpenPopup(object sender, EventArgs e)
        {
            var notifyIconRect = _notifyIcon.GetRect();
            ShowTooltipPopup(notifyIconRect);
        }

        private void NotifyIcon_ClosePopup(object sender, EventArgs e)
        {
            HideTooltipPopup();
        }

        public void ShowContextMenu(Point offset, Rectangle notifyIconRect)
        {
            if (View.ContextFlyout is MenuFlyout menu)
            {
                if (menu.IsOpen)
                {
                    menu.Hide();
                    return;
                }

                SetForegroundWindow();

                X = notifyIconRect.X;
                Y = notifyIconRect.Y;
                Width = notifyIconRect.Width;
                Height = notifyIconRect.Height;
                SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

                var position = new Windows.Foundation.Point(
                        (offset.X - notifyIconRect.X) / ScaleFactor,
                        (offset.Y - notifyIconRect.Y) / ScaleFactor);
                menu.ShowAt(View, position);
            }
        }

        public void ShowTooltipPopup(Rectangle notifyIconRect)
        {
            X = notifyIconRect.X;
            Y = notifyIconRect.Y;
            Width = notifyIconRect.Width;
            Height = notifyIconRect.Height;
            SetWindowPos(IntPtr.Zero, SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOACTIVATE);

            var tooltip = (ToolTip)ToolTipService.GetToolTip(View);
            tooltip.IsOpen = true;
        }

        public void HideTooltipPopup()
        {
            var tooltip = (ToolTip)ToolTipService.GetToolTip(View);
            tooltip.IsOpen = false;
        }
    }
}
