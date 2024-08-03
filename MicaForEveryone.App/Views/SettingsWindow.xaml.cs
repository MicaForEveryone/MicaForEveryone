using MicaForEveryone.App.ViewModels;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;
using WinRT.Interop;
using static MicaForEveryone.PInvoke.Messaging;
using static MicaForEveryone.PInvoke.Modules;
using static MicaForEveryone.PInvoke.Windowing;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public unsafe sealed partial class SettingsWindow : Window
{
    private static delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> oldWndProc;

    private SettingsViewModel ViewModel { get; }

    private MenuFlyout _addNewItemFlyout = new();

    public SettingsWindow()
    {
        InitializeComponent();

        MenuFlyoutItem processRuleItem = new() { Text = "_Add Process Rule" };
        processRuleItem.Click += (_, _) =>
        {
            AddProcessRuleContentDialog addProcessRuleContentDialog = new();
            addProcessRuleContentDialog.XamlRoot = Content.XamlRoot;
            _ = addProcessRuleContentDialog.ShowAsync();
        };
        _addNewItemFlyout.Items.Add(processRuleItem);

        MenuFlyoutItem classRuleItem = new() { Text = "_Add Class Rule" };
        classRuleItem.Click += (_, _) =>
        {
            AddClassRuleContentDialog addClassRuleContentDialog = new();
            addClassRuleContentDialog.XamlRoot = Content.XamlRoot;
            _ = addClassRuleContentDialog.ShowAsync();
        };
        _addNewItemFlyout.Items.Add(classRuleItem);

        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
        ExtendsContentIntoTitleBar = true;

        Title = "_Mica For Everyone Settings";

        HICON icon;
        LoadIconMetric(GetModuleHandleW(null), IDI_APPLICATION, LIM_LARGE, &icon);

        AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(new nint(icon.Value)));

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

    private void Window_Activated(object _, WindowActivatedEventArgs args)
    {
        // TODO: Add code to deal with title bar color change.
    }

    private void NavigationViewControl_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is Rule rule)
        {
            _contentFrame.Navigate(typeof(RuleSettingsPage), rule);
        }
        else
        {
            _contentFrame.Navigate(typeof(AppSettingsPage));
        }
    }

    private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer.Tag is SettingsNavigationItem { Tag: "AddRuleNavViewItem" })
        {
            _addNewItemFlyout.ShowAt(args.InvokedItemContainer);
        }
    }
}

public class SettingsNavigationItem
{
    public string? Uid { get; set; }

    public string? Tag { get; set; }

    public IconElement? Icon { get; set; }

    public bool Selectable { get; set; } = true;
}