using MicaForEveryone.App.Controls.Navigation;
using MicaForEveryone.App.ViewModels;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics.CodeAnalysis;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ShellPage : Page
{
    private SettingsViewModel ViewModel { get; }

    public ShellPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
    }

    private void TitleBarControl_PaneToggleRequested(TitleBar sender, object args)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }

    public void ShowAppSettings()
    {
        NavigationViewControl.SelectedItem = AppSettingsItem;
        AppSettingsItem.IsSelected = true;
        _contentFrame.Navigate(typeof(AppSettingsPage));
    }

    private void RuleListView_SelectionChanged(Controls.Navigation.NavigationView sender, Controls.Navigation.NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is not MicaForEveryone.Models.Rule)
        {
            _contentFrame.Navigate(typeof(AppSettingsPage));
            return;
        }
        _contentFrame.Navigate(typeof(RuleSettingsPage), e.SelectedItem);
    }

    private void Page_Unloaded(object sender, RoutedEventArgs e)
    {
        Unloaded -= Page_Unloaded;
        _contentFrame.Navigated -= _contentFrame_Navigated;
        TitleBarControl.PaneToggleRequested -= TitleBarControl_PaneToggleRequested;
        NavigationViewControl.SelectionChanged -= RuleListView_SelectionChanged;
        Bindings?.StopTracking();

        GC.Collect(2, GCCollectionMode.Forced, false, false);
    }

    private void _contentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        // Released bindings are located in Gen2 GC, which means
        // we have to free it manually, since automatic Gen2 collection
        // happens very rarely, or we will experience "infinite" memory growth.
        GC.Collect(2, GCCollectionMode.Forced, false, false);
    }

    private void AddProcessRuleMenuFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        AddProcessRuleContentDialog addProcessRuleContentDialog = new();
        addProcessRuleContentDialog.XamlRoot = XamlRoot;
        _ = addProcessRuleContentDialog.ShowAsync();
    }

    private void AddClassRuleMenuFlyoutItem_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        AddClassRuleContentDialog addClassRuleContentDialog = new();
        addClassRuleContentDialog.XamlRoot = XamlRoot;
        _ = addClassRuleContentDialog.ShowAsync();
    }
}

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

        throw new System.Diagnostics.UnreachableException("Navigation menu item type is invalid.");
    }
}
