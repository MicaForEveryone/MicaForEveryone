using MicaForEveryone.App.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Foundation;
using WinUIEx;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Macros;
using static MicaForEveryone.PInvoke.Modules;
using static MicaForEveryone.PInvoke.Monitor;
using static MicaForEveryone.PInvoke.NotifyIcon;
using static MicaForEveryone.PInvoke.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public unsafe partial class App : Application
{
    static DesktopWindowXamlSource _source;
    static object lockObject = new();
    static SettingsWindow? window;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        HINSTANCE instance = GetModuleHandleW(null);
        fixed (char* lpClassName = "MicaForEveryoneNotificationIcon")
        {
            WNDCLASSEXW wndClass = new()
            {
                cbSize = (uint)sizeof(WNDCLASSEXW),
                style = WindowClassStyles.CS_HREDRAW | WindowClassStyles.CS_VREDRAW,
                lpfnWndProc = &WindowProc,
                hInstance = instance,
                hCursor = HCURSOR.NULL,
                lpszClassName = (ushort*)lpClassName,
                lpszMenuName = null,
                hIcon = LoadIconW(instance, IDI_APPLICATION),
                hIconSm = LoadIconW(instance, IDI_APPLICATION),
                cbClsExtra = 0,
                cbWndExtra = 0,
                hbrBackground = HBRUSH.NULL
            };

            RegisterClassExW(&wndClass);
        }

        HWND window = CreateWindowExW(WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TOPMOST, "MicaForEveryoneNotificationIcon", null, WindowStyles.WS_POPUPWINDOW, 0, 0, 0, 0, HWND.NULL, null, instance, null);

        // We have to show the window, or it crashes.
        ShowWindow(window, 5);
    }

    public static void ActivateSettings()
    {
        lock (lockObject)
        {
            if (window == null)
            {
                window = new SettingsWindow();
                window.Closed += (_, _) =>
                {
                    lock (lockObject)
                    {
                        window = null;
                    }
                };
                window.Activate();
            }
            else
                window.BringToFront();
        }
    }

    [UnmanagedCallersOnly]
    private static LRESULT WindowProc(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam)
    {
        if (Msg == 1)
        {
            _source = new();
            var thing = Win32Interop.GetWindowIdFromWindow(new IntPtr(hWnd.Value));
            _source.Initialize(thing);
            _source.Content = new TrayIconPage();
            _source.SiteBridge.ResizePolicy = Microsoft.UI.Content.ContentSizePolicy.ResizeContentToParentWindow;
            _source.SiteBridge.Show();

            NOTIFYICONDATAW notifyIconData = new();
            notifyIconData.hWnd = hWnd;
            notifyIconData.uID = 1;
            notifyIconData.cbSize = (uint)sizeof(NOTIFYICONDATAW);
            notifyIconData.hIcon = LoadIconW(GetModuleHandleW(null), IDI_APPLICATION);
            notifyIconData.uVersion = 4;
            notifyIconData.uCallbackMessage = WM_APP + 1;

            // Currently, we can't show a tool tip for the app name,
            // so we just tell Windows to show it for us.
            // It might look a bit ugly, but it works.
            notifyIconData.uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP | NIF_SHOWTIP;
            "Mica For Everyone".CopyTo(MemoryMarshal.Cast<ushort, char>(MemoryMarshal.CreateSpan(ref notifyIconData.szTip[0], 128)));
            Shell_NotifyIconW(NIM_ADD, &notifyIconData);
            Shell_NotifyIconW(NIM_SETVERSION, &notifyIconData);
        }
        if (Msg == WM_APP + 1)
        {
            RECT iconRect;
            NOTIFYICONIDENTIFIER id = new NOTIFYICONIDENTIFIER()
            {
                cbSize = (uint)sizeof(NOTIFYICONIDENTIFIER),
                uID = 1,
                hWnd = hWnd
            };
            Shell_NotifyIconGetRect(&id, &iconRect);
            HMONITOR monitor = MonitorFromRect(&iconRect, MONITOR_DEFAULTTONULL);
            MONITORINFO monitorInfo;
            bool monitorSuccessful = false;
            int? workBottom = null;

            if (monitor != HMONITOR.NULL && (monitorSuccessful = GetMonitorInfoW(monitor, &monitorInfo)))
                if ((workBottom = monitorInfo.rcWork.bottom) < iconRect.bottom)
                    iconRect.top = iconRect.bottom - 1;

            SetWindowPos(hWnd, HWND.NULL, iconRect.left, iconRect.top, 0, 0, SWP_NOACTIVATE | SWP_NOZORDER);

            switch (LOWORD(lParam))
            {
                case 0x007B:
                    nint handleIntPtr = new nint(hWnd.Value);
                    var scaleFactor = HwndExtensions.GetDpiForWindow(handleIntPtr) / 96f;
                    HwndExtensions.SetForegroundWindow(handleIntPtr);

                    Point point = new(
                        GET_X_LPARAM(new((nint)wParam.Value)),
                        GET_Y_LPARAM(new((nint)wParam.Value))
                    );

                    if (workBottom != null && workBottom < point.Y)
                        point.Y = workBottom.Value - 1;

                    point = new(
                        (point.X - iconRect.left) / scaleFactor,
                        (point.Y - iconRect.top) / scaleFactor
                    );

                    var page = (TrayIconPage)_source.Content;
                    ((MenuFlyout)page.ContextFlyout).ShowAt(page, point);
                    break;

                case NIN_SELECT:
                case NIN_KEYSELECT:
                case WM_LBUTTONUP:
                    ActivateSettings();
                    break;
            }
        }
        return DefWindowProcW(hWnd, Msg, wParam, lParam);
    }
}
