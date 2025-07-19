using MicaForEveryone.App.Views;
using Microsoft.UI;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using System;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using Windows.Foundation;
using WinRT;
using WinRT.Interop;
using static TerraFX.Interop.Windows.Windows;

namespace MicaForEveryone.App.Services;

public sealed unsafe class MainAppService
{
    XamlIsland? _source;
    HWND _mainWnd;
    SettingsWindow? _window;
    private uint _taskbarCreatedMessage;
    private NOTIFYICONDATAW* _notifyIconData;

    // MODIFICATION: New public method to get XamlRoot for dialogs
    public XamlRoot? GetMainWindowXamlRoot()
    {
        // The tray icon is hosted in a XamlIsland within a hidden window.
        // We can use its XamlRoot to show dialogs.
        if (_source?.Content != null)
        {
            return _source.Content.XamlRoot;
        }
        // If the settings window is open, we can use that too.
        if (_window?.Content != null)
        {
            return _window.Content.XamlRoot;
        }
        return null;
    }

    public void Initialize()
    {
        HINSTANCE instance = GetModuleHandleW(null);
        HICON largeIcon, smallIcon;
        LoadIconMetric(instance, IDI.IDI_APPLICATION, (int)_LI_METRIC.LIM_LARGE, &largeIcon);
        LoadIconMetric(instance, IDI.IDI_APPLICATION, (int)_LI_METRIC.LIM_SMALL, &smallIcon);

        fixed (char* lpClassName = "MicaForEveryoneNotificationIcon")
        {
            WNDCLASSEXW wndClass = new() { cbSize = (uint)sizeof(WNDCLASSEXW), style = CS.CS_HREDRAW | CS.CS_VREDRAW, lpfnWndProc = &WindowProc, hInstance = instance, hCursor = HCURSOR.NULL, lpszClassName = lpClassName, lpszMenuName = null, hIcon = largeIcon, hIconSm = smallIcon, cbClsExtra = 0, cbWndExtra = 0, hbrBackground = HBRUSH.NULL };
            RegisterClassExW(&wndClass);
        }
        nint gcHandlePtr = GCHandle.ToIntPtr(GCHandle.Alloc(this));

        fixed (char* lpWindowTitle = "MicaForEveryoneNotificationIcon")
        {
            _mainWnd = CreateWindowExW(WS.WS_EX_NOACTIVATE | WS.WS_EX_TOPMOST | WS.WS_EX_TOOLWINDOW, lpWindowTitle, null, WS.WS_POPUPWINDOW, 0, 0, 0, 0, HWND.NULL, HMENU.NULL, instance, gcHandlePtr.ToPointer());
        }
        var rgn = CreateRectRgn(0, 0, 0, 0);
        SetWindowRgn(_mainWnd, rgn, false);
        ShowWindow(_mainWnd, 5);

        fixed (char* lpWindowMessage = "TaskbarCreated")
        {
            _taskbarCreatedMessage = RegisterWindowMessageW(lpWindowMessage);
        }
    }

    public void ActivateSettings()
    {
        if (_window == null)
        {
            _window = new SettingsWindow();
            _window.Closed += _window_Closed;
            _window.Activate();
            HWND hwnd = new HWND((void*)WindowNative.GetWindowHandle(_window));
            SetForegroundWindow(hwnd);
        }
        else
        {
            _window.Activate();
        }
    }

    private void _window_Closed(object sender, WindowEventArgs args)
    {
        ((Window)sender).Closed -= _window_Closed;
        _window = null;
    }

    public void Shutdown()
    {
        _window?.Close();
        DestroyWindow(_mainWnd);
    }

    [UnmanagedCallersOnly]
    private static LRESULT WindowProc(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam)
    {
        switch (Msg)
        {
            case WM.WM_CREATE:
            {
                HICON smallIcon;
                LoadIconMetric(GetModuleHandleW(null), IDI.IDI_APPLICATION, (int)_LI_METRIC.LIM_SMALL, &smallIcon);

                CREATESTRUCTW* lpCreateStruct = (CREATESTRUCTW*)lParam;
                nint gcHandlePtr = lpCreateStruct->lpCreateParams;
                var gc = GCHandle.FromIntPtr(gcHandlePtr);
                var appService = (MainAppService)(gc.Target!);
                appService._source = new XamlIsland();
                var thing = Win32Interop.GetWindowIdFromWindow(new IntPtr(hWnd.Value));

                DesktopChildSiteBridge bridge = DesktopChildSiteBridge.CreateWithDispatcherQueue(DispatcherQueue.GetForCurrentThread(), thing);
                appService._source.Content = new TrayIconPage();
                bridge.Connect(appService._source.ContentIsland);
                bridge.ResizePolicy = ContentSizePolicy.ResizeContentToParentWindow;
                bridge.Show();

                SetWindowLongPtrW(hWnd, GWL.GWL_USERDATA, gcHandlePtr);

                appService._notifyIconData = (NOTIFYICONDATAW*)NativeMemory.AllocZeroed((nuint)sizeof(NOTIFYICONDATAW));
                appService._notifyIconData->hWnd = hWnd;
                appService._notifyIconData->guidItem = new Guid("A0235A9F-C6B6-4189-AE4B-AC009FC6787C");
                appService._notifyIconData->cbSize = (uint)sizeof(NOTIFYICONDATAW);
                appService._notifyIconData->hIcon = smallIcon;
                appService._notifyIconData->uVersion = 4;
                appService._notifyIconData->uCallbackMessage = WM.WM_APP + 1;
                appService._notifyIconData->uFlags = NIF_ICON | NIF_MESSAGE | NIF_TIP | NIF_GUID;
                "Mica For Everyone".CopyTo(MemoryMarshal.CreateSpan(ref appService._notifyIconData->szTip[0], 128));
                Shell_NotifyIconW(NIM_ADD, appService._notifyIconData);
                Shell_NotifyIconW(NIM_SETVERSION, appService._notifyIconData);
                break;
            }
            case WM.WM_APP + 1:
            {
                var pointer = GetWindowLongPtrW(hWnd, GWL.GWL_USERDATA);
                var gc = GCHandle.FromIntPtr(pointer);
                var appService = (MainAppService)(gc.Target!);

                if (LOWORD(lParam) is WM.WM_CONTEXTMENU or WM.WM_RBUTTONUP)
                {
                    RECT iconRect;
                    NOTIFYICONIDENTIFIER id = new() { cbSize = (uint)sizeof(NOTIFYICONIDENTIFIER), hWnd = hWnd, guidItem = appService._notifyIconData->guidItem };
                    Shell_NotifyIconGetRect(&id, &iconRect);

                    SetForegroundWindow(hWnd);

                    var page = (TrayIconPage)(appService._source!.Content);
                    page.ContextFlyout.ShowAt(null, new Point(iconRect.left, iconRect.top));
                }
                else if (LOWORD(lParam) is NIN_SELECT or NIN_KEYSELECT)
                {
                    appService.ActivateSettings();
                }
                break;
            }
            case WM.WM_DESTROY:
            {
                var pointer = GetWindowLongPtrW(hWnd, GWL.GWL_USERDATA);
                var gc = GCHandle.FromIntPtr(pointer);
                var appService = (MainAppService)(gc.Target!);

                if (appService._notifyIconData != null)
                {
                    Shell_NotifyIconW(NIM_DELETE, appService._notifyIconData);
                    NativeMemory.Free(appService._notifyIconData);
                }

                appService._source?.Dispose();
                appService._source = null;
                gc.Free();
                PostQuitMessage(0);
                break;
            }
            default:
            {
                var pointer = GetWindowLongPtrW(hWnd, GWL.GWL_USERDATA);
                if (pointer == IntPtr.Zero)
                    return DefWindowProcW(hWnd, Msg, wParam, lParam);
                var gc = GCHandle.FromIntPtr(pointer);
                var appService = (MainAppService?)(gc.Target);
                if (appService != null && Msg == appService._taskbarCreatedMessage)
                {
                    if (!Shell_NotifyIconW(NIM_MODIFY, appService._notifyIconData))
                    {
                        Shell_NotifyIconW(NIM_ADD, appService._notifyIconData);
                        Shell_NotifyIconW(NIM_SETVERSION, appService._notifyIconData);
                    }
                    return 0;
                }
                break;
            }
        }
        return DefWindowProcW(hWnd, Msg, wParam, lParam);
    }
}
