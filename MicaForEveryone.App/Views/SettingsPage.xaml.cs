using MicaForEveryone.App.ViewModels;
using MicaForEveryone.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        ChangeButtonBackground();
        OwnerWindow?.SetTitleBar(TitleBarControl);
    }

    private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
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

    private void ChangeButtonBackground()
    {
        OwnerWindow?.AppWindow.TitleBar.ButtonHoverBackgroundColor = (Color) Application.Current.Resources["SubtleFillColorSecondary"];
        OwnerWindow?.AppWindow.TitleBar.ButtonPressedBackgroundColor = (Color) Application.Current.Resources["SubtleFillColorTertiary"];
        OwnerWindow?.AppWindow.TitleBar.ButtonForegroundColor = OwnerWindow?.AppWindow.TitleBar.ButtonHoverForegroundColor = (Color) Application.Current.Resources["TextFillColorPrimary"];
    }

    private void RootPage_ActualThemeChanged(FrameworkElement sender, object args) => ChangeButtonBackground();

    private void RootPage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width < 700)
        {
            OwnerWindow?.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Tall;
        }
        else
        {
            OwnerWindow?.AppWindow.TitleBar.PreferredHeightOption = Microsoft.UI.Windowing.TitleBarHeightOption.Standard;
        }
    }

    private void TitleBarControl_PaneToggleRequested(TitleBar sender, object args)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }

    public Window? OwnerWindow { get; set; }

    private readonly SettingsViewModel ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
}

public sealed class AddNewRuleMenuItem;

public sealed class AppSettingsMenuItem;

public sealed partial class SettingsNavigationItemSelector : DataTemplateSelector
{
    public DataTemplate GlobalRuleTemplate { get; set; } = new();

    public DataTemplate ProcessRuleTemplate { get; set; } = new();

    public DataTemplate ClassRuleTemplate { get; set; } = new();

    public DataTemplate AddNewRuleTemplate { get; set; } = new();

    public DataTemplate AppSettingsTemplate { get; set; } = new();

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is GlobalRule)
            return GlobalRuleTemplate;

        if (item is ProcessRule)
            return ProcessRuleTemplate;

        if (item is ClassRule)
            return ClassRuleTemplate;

        if (item is AddNewRuleMenuItem)
            return AddNewRuleTemplate;

        if (item is AppSettingsMenuItem)
            return AppSettingsTemplate;

        throw new ArgumentException("Navigation menu item type is invalid.");
    }
}
