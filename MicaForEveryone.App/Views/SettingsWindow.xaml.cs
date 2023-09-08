using MicaForEveryone.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using WinRT.Interop;
using WinUIEx;
using static MicaForEveryone.PInvoke.Messaging;
using static MicaForEveryone.PInvoke.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public unsafe sealed partial class SettingsWindow : WindowEx
{
    private static delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> oldWndProc;
    private SettingsViewModel ViewModel { get; }

    public SettingsWindow()
    {
        InitializeComponent();

        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
        ExtendsContentIntoTitleBar = true;

        oldWndProc = (delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)SetWindowLongPtrW(new HWND((void*)WindowNative.GetWindowHandle(this)), WindowLongIndex.GWL_WNDPROC, (nint)(delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)&WindowProc);
    }

    [UnmanagedCallersOnly]
    private static LRESULT WindowProc(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam)
    {
        if (Msg == WM_DESTROY)
        {
            LRESULT result = CallWindowProcW(oldWndProc, hWnd, Msg, wParam, lParam);
            MSG msg;
            while (PeekMessageW(&msg, default, 0, 0, 1))
            {
                if (msg.message != 18)
                {
                    TranslateMessage(&msg);
                    DispatchMessageW(&msg);
                    continue;
                }
                break;
            }
            return result;
        }
        return CallWindowProcW(oldWndProc, hWnd, Msg, wParam, lParam);
    }
}
