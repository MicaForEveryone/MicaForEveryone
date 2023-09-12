using MicaForEveryone.App.Views;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using Windows.Foundation;
using WinUIEx;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Macros;
using static MicaForEveryone.PInvoke.Messaging;
using static MicaForEveryone.PInvoke.Modules;
using static MicaForEveryone.PInvoke.Monitor;
using static MicaForEveryone.PInvoke.NotifyIcon;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.App.Services;

public sealed unsafe class MainAppService
{
    DesktopWindowXamlSource? _source;
    object _lockObject = new();
    SettingsWindow? _window;
    HWND _mainWnd;

    public void Initialize()
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
        nint gcHandlePtr = GCHandle.ToIntPtr(GCHandle.Alloc(this));
        _mainWnd = CreateWindowExW(WindowStylesEx.WS_EX_NOACTIVATE | WindowStylesEx.WS_EX_TOPMOST, "MicaForEveryoneNotificationIcon", null, WindowStyles.WS_POPUPWINDOW, 0, 0, 0, 0, HWND.NULL, null, instance, gcHandlePtr.ToPointer());
        // We have to show the window, or it crashes.
        ShowWindow(_mainWnd, 5);
    }

    public void ActivateSettings()
    {
        lock (_lockObject)
        {
            if (_window == null)
            {
                _window = new SettingsWindow();
                _window.Closed += (_, _) =>
                {
                    lock (_lockObject)
                    {
                        _window = null;
                    }
                };
                _window.Activate();
            }
            else
            {
                var hwnd = new HWND((void*)_window.GetWindowHandle());
                if (IsIconic(hwnd))
                {
                    ShowWindow(hwnd, SW_RESTORE);
                }
                _window.BringToFront();
            }
        }
    }

    public void Shutdown()
    {
        lock (_lockObject)
        {
            _window?.Close();
            DestroyWindow(_mainWnd);
        }
    }

    [UnmanagedCallersOnly]
    private static LRESULT WindowProc(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam)
    {
        switch (Msg)
        {
            case 1:
                {
                    CREATESTRUCTW* lpCreateStruct = (CREATESTRUCTW*)&lParam;
                    nint gcHandlePtr = *(nint*)lpCreateStruct->lpCreateParams;
                    var gc = GCHandle.FromIntPtr(gcHandlePtr);
                    var appService = Unsafe.As<MainAppService>(gc.Target!);
                    appService._source = new();
                    var thing = Win32Interop.GetWindowIdFromWindow(new IntPtr(hWnd.Value));
                    appService._source.Initialize(thing);
                    appService._source.Content = new TrayIconPage();
                    appService._source.SiteBridge.ResizePolicy = Microsoft.UI.Content.ContentSizePolicy.ResizeContentToParentWindow;
                    appService._source.SiteBridge.Show();

                    SetWindowLongPtrW(hWnd, WindowLongIndex.GWL_USERDATA, gcHandlePtr);

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
                    break;
                }

            case WM_APP + 1:
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

                    var pointer = GetWindowLongPtrW(hWnd, WindowLongIndex.GWL_USERDATA);
                    var gc = GCHandle.FromIntPtr(pointer);
                    var appService = Unsafe.As<MainAppService>(gc.Target!);

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

                            point = new(
                                (point.X - iconRect.left) / scaleFactor,
                                (point.Y - iconRect.top) / scaleFactor
                            );

                            var page = Unsafe.As<TrayIconPage>(appService._source!.Content);
                            (Unsafe.As<MenuFlyout>(page.ContextFlyout)).ShowAt(page, point);
                            break;

                        case NIN_SELECT:
                        case NIN_KEYSELECT:
                        case WM_LBUTTONUP:
                            appService.ActivateSettings();
                            break;
                    }
                    break;
                }

            case WM_DESTROY:
                {
                    NOTIFYICONDATAW notifyIconData = new();
                    notifyIconData.hWnd = hWnd;
                    notifyIconData.uID = 1;
                    notifyIconData.cbSize = (uint)sizeof(NOTIFYICONDATAW);
                    Shell_NotifyIconW(NIM_DELETE, &notifyIconData);

                    var pointer = GetWindowLongPtrW(hWnd, WindowLongIndex.GWL_USERDATA);
                    var gc = GCHandle.FromIntPtr(pointer);
                    var appService = Unsafe.As<MainAppService>(gc.Target!);
                    appService._source?.Dispose();
                    appService._source = null;

                    gc.Free();

                    PostQuitMessage(0);
                    break;
                }
        }
        return DefWindowProcW(hWnd, Msg, wParam, lParam);
    }
}
