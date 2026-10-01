using MicaForEveryone.App.Controls.Navigation;
using MicaForEveryone.App.Services;
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
    [DynamicWindowsRuntimeCast(typeof(OverlappedPresenter))]
    public SettingsWindow()
    {
        this.InitializeComponent();

        var LocalizationService = App.Services.GetRequiredService<ILocalizationService>();

        ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        ChangeButtonBackground();
        Title = LocalizationService.GetLocalizedString("SettingsWindowTitle");
        AppWindow.SetIcon("Assets\\MicaForEveryone.ico");

        unsafe
        {
            HWND hwnd = new HWND((void*)WinRT.Interop.WindowNative.GetWindowHandle(this));
            SetWindowSubclass(hwnd, &WindowProc, 0, 0);

            uint dpi = GetDpiForWindow(hwnd);

            int width = (int)(900 * dpi / 96.0f);
            int height = (int)(600 * dpi / 96.0f);

            int min = (int)(500 * dpi / 96.0f);

            OverlappedPresenter presenter = (OverlappedPresenter)AppWindow.Presenter;
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
        if (arg2 == WM.WM_NCDESTROY)
        {
            RemoveWindowSubclass(hWND, &WindowProc, 0);
        }
        return DefSubclassProc(hWND, arg2, wPARAM, lPARAM);
    }

    

    public void ShowAppSettings() => RootPage.ShowAppSettings();

    private void ChangeButtonBackground()
    {
        AppWindow.TitleBar.ButtonHoverBackgroundColor = (Color)Application.Current.Resources["SubtleFillColorSecondary"];
        AppWindow.TitleBar.ButtonPressedBackgroundColor = (Color)Application.Current.Resources["SubtleFillColorTertiary"];
        AppWindow.TitleBar.ButtonForegroundColor = AppWindow.TitleBar.ButtonHoverForegroundColor = (Color)Application.Current.Resources["TextFillColorPrimary"];
    }

    private void RootPage_ActualThemeChanged(FrameworkElement sender, object args) => ChangeButtonBackground();

    private void RootPage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width < 700)
        {
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        }
        else
        {
            AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;
        }
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        Closed -= Window_Closed;
        RootPage.ActualThemeChanged -= RootPage_ActualThemeChanged;
        RootPage.SizeChanged -= RootPage_SizeChanged;
    }
}
