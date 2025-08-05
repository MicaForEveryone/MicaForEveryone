using MicaForEveryone.App.ViewModels;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using Windows.UI;
using WinRT;
using static TerraFX.Interop.Windows.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsWindow : Window
{
    private ILocalizationService LocalizationService { get; }

    public SettingsWindow()
    {
        this.InitializeComponent();

        LocalizationService = App.Services.GetRequiredService<ILocalizationService>();

        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        Title = LocalizationService.GetLocalizedString("SettingsWindowTitle");
        AppWindow.SetIcon("Assets\\MicaForEveryone.ico");

        RootPage.OwnerWindow = this;

        unsafe
        {
            HWND hwnd = new HWND((void*)WinRT.Interop.WindowNative.GetWindowHandle(this));
            SetWindowSubclass(hwnd, &WindowProc, 0, 0);

            uint dpi = GetDpiForWindow(hwnd);

            int width = (int)(900 * dpi / 96.0f);
            int height = (int)(600 * dpi / 96.0f);

            int min = (int)(500 * dpi / 96.0f);

            OverlappedPresenter presenter = AppWindow.Presenter.As<OverlappedPresenter>();
            presenter.PreferredMinimumWidth = presenter.PreferredMinimumHeight = min;

            AppWindow.Resize(new Windows.Graphics.SizeInt32(width, height));

            RECT rcWorkArea;
            SystemParametersInfo(SPI.SPI_GETWORKAREA, 0, &rcWorkArea, 0);
            int x = (int)((rcWorkArea.right + rcWorkArea.left) / 2.0f - width / 2.0f);
            int y = (int)((rcWorkArea.top + rcWorkArea.bottom) / 2.0f - height / 2.0f);

            AppWindow.Move(new Windows.Graphics.PointInt32(x, y));
        }
        // NavigationViewControl.SelectedItem = NavigationViewControl.FooterMenuItems.Last();
    }

    [UnmanagedCallersOnly]
    private static unsafe LRESULT WindowProc(HWND hWND, uint arg2, WPARAM wPARAM, LPARAM lPARAM, nuint arg5, nuint arg6)
    {
        if (arg2 == WM.WM_DESTROY)
        {
            LRESULT result = DefSubclassProc(hWND, arg2, wPARAM, lPARAM);
            MSG msg;
            while (PeekMessage(&msg, HWND.NULL, 0, 0, PM.PM_REMOVE))
            {
                if (msg.message != WM.WM_QUIT)
                {
                    TranslateMessage(&msg);
                    DispatchMessage(&msg);
                    continue;
                }
                break;
            }
            return result;
        }
        /*
        if (arg2 == WM.WM_GETMINMAXINFO)
        {
            var dpi = GetDpiForWindow(hWND);
            float scale = dpi / 96.0f;
            MINMAXINFO* minMaxInfo = (MINMAXINFO*)lPARAM;
            minMaxInfo->ptMinTrackSize.x = (int)(500 * scale);
            minMaxInfo->ptMinTrackSize.y = (int)(500 * scale);
        }
        */
        return DefSubclassProc(hWND, arg2, wPARAM, lPARAM);
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        /*
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            try
            {
                // This may cause an exception on closing, not sure why.
                VisualStateManager.GoToState(RootPage, "TitleBarInactivated", false);
            }
            catch { }
        }
        else
        {
            VisualStateManager.GoToState(RootPage, "TitleBarActive", false);
        }
        */
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        Activated -= Window_Activated;
    }
}
