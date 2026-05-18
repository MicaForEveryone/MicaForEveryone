using MicaForEveryone.App.Controls.Navigation;
using MicaForEveryone.App.Helpers;
using MicaForEveryone.App.ViewModels;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TerraFX.Interop.Windows;
using WinRT;
using NavigationViewItem = MicaForEveryone.App.Controls.Navigation.NavigationViewItem;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

public sealed partial class ShellPage : Page
{
    private SettingsViewModel ViewModel { get; }
    private readonly HashSet<NavigationViewItem> _eventAttachedItems = new();

    public ShellPage()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();

        NavigationViewControl.Loaded += NavigationViewControl_Loaded;
    }

    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        NavigationViewControl.Loaded -= NavigationViewControl_Loaded;
        LoadRuleIcons();
        AttachRuleItemEvents();
    }

    private void TitleBarControl_PaneToggleRequested(TitleBar sender, object args)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
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

        // 解绑所有动态附加的事件
        foreach (var item in _eventAttachedItems)
        {
            item.DoubleTapped -= RuleItem_DoubleTapped;
            item.RightTapped -= RuleItem_RightTapped;
        }
        _eventAttachedItems.Clear();

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

    private async void AddProcessRuleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var rules = ViewModel.SettingsService.Settings!.Rules;
        AddProcessRuleContentDialog dialog = new();
        dialog.XamlRoot = XamlRoot;
        await dialog.ShowAsync();

        if (rules.Count > 1)
        {
            var newRule = rules[1];
            if (!string.IsNullOrEmpty(newRule.IconPath))
                LoadIconForNewRule(newRule);
        }

        AttachEventsAfterLayout();
    }

    private async void AddClassRuleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        var rules = ViewModel.SettingsService.Settings!.Rules;
        AddClassRuleContentDialog dialog = new();
        dialog.XamlRoot = XamlRoot;
        await dialog.ShowAsync();

        if (rules.Count > 1)
        {
            var newRule = rules[1];
            if (!string.IsNullOrEmpty(newRule.IconPath))
                LoadIconForNewRule(newRule);
        }

        AttachEventsAfterLayout();
    }

    private void LoadIconForNewRule(Rule rule)
    {
        int retryCount = 0;
        const int maxRetries = 10;
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        EventHandler<object>? onLayoutUpdated = null;
        onLayoutUpdated = (sender, args) =>
        {
            retryCount++;
            if (TryLoadIconForRule(rule) || retryCount >= maxRetries)
            {
                NavigationViewControl.LayoutUpdated -= onLayoutUpdated;
                cts.Dispose();
            }
        };

        NavigationViewControl.LayoutUpdated += onLayoutUpdated;

        cts.Token.Register(() =>
        {
            NavigationViewControl.LayoutUpdated -= onLayoutUpdated;
        });
    }

    private void AttachEventsAfterLayout()
    {
        EventHandler<object>? handler = null;
        handler = (s, e) =>
        {
            NavigationViewControl.LayoutUpdated -= handler;
            AttachRuleItemEvents();
        };
        NavigationViewControl.LayoutUpdated += handler;
    }

    private bool TryLoadIconForRule(Rule rule)
    {
        if (string.IsNullOrEmpty(rule.IconPath))
            return true;

        var navItem = FindVisualChildren<NavigationViewItem>(NavigationViewControl)
            .FirstOrDefault(item => item.DataContext == rule);
        if (navItem == null)
            return false;

        try
        {
            using var icon = System.Drawing.Icon.ExtractAssociatedIcon(rule.IconPath);
            if (icon != null)
            {
                var bitmapImage = IconHelper.IconToBitmapImage(icon);
                navItem.Icon = new ImageIcon { Source = bitmapImage };
                return true;
            }
        }
        catch { }
        return false;
    }

    private void LoadRuleIcons()
    {
        foreach (var rule in ViewModel.SettingsService.Settings!.Rules)
        {
            if (!string.IsNullOrEmpty(rule.IconPath))
                TryLoadIconForRule(rule);
        }
    }

    private void AttachRuleItemEvents()
    {
        var items = FindVisualChildren<NavigationViewItem>(NavigationViewControl);
        foreach (var item in items)
        {
            if (item.DataContext is Rule rule && rule is not GlobalRule)
            {
                AttachEventsToItem(item);
            }
        }
    }

    private void AttachEventsToItem(NavigationViewItem item)
    {
        if (_eventAttachedItems.Contains(item)) return;

        item.DoubleTapped += RuleItem_DoubleTapped;
        item.RightTapped += RuleItem_RightTapped;
        _eventAttachedItems.Add(item);
    }

    private async void RuleItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is not NavigationViewItem navItem || navItem.DataContext is not Rule rule)
            return;

        if (rule is not ProcessRule processRule || string.IsNullOrEmpty(processRule.ProcessName))
            return;

        var processes = Process.GetProcessesByName(processRule.ProcessName);
        var targetProcess = processes.FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);
        if (targetProcess == null) return;

        HWND hwnd;
        unsafe { hwnd = new HWND((void*)targetProcess.MainWindowHandle); }

        var icon = await IconHelper.ExtractIconFromWindowAsync(hwnd);
        if (icon != null)
        {
            using (icon)
            {
                string? iconPath = IconHelper.GetLastExtractedPath();
                if (!string.IsNullOrEmpty(iconPath))
                {
                    rule.IconPath = iconPath;
                    await ViewModel.SettingsService.SaveAsync();
                    var bitmapImage = IconHelper.IconToBitmapImage(icon);
                    navItem.Icon = new ImageIcon { Source = bitmapImage };
                }
            }
        }
    }

    private async void RuleItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        if (sender is not NavigationViewItem navItem || navItem.DataContext is not Rule rule)
            return;
        if (string.IsNullOrEmpty(rule.IconPath))
            return;

        rule.IconPath = null;
        await ViewModel.SettingsService.SaveAsync();

        if (rule is ProcessRule)
            navItem.Icon = new FontIcon { Glyph = "\uECAA" };
        else if (rule is ClassRule)
            navItem.Icon = new FontIcon { Glyph = "\uE737" };
    }

    [DynamicWindowsRuntimeCast(typeof(FrameworkElement))]
    private static T? FindChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild && (typedChild as FrameworkElement)?.Name == name)
                return typedChild;

            var result = FindChildByName<T>(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) yield break;
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T tChild)
                yield return tChild;

            foreach (var descendant in FindVisualChildren<T>(child))
                yield return descendant;
        }
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