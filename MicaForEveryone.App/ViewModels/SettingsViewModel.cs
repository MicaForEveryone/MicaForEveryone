using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.App.Views;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.UI.Xaml.Controls;

namespace MicaForEveryone.App.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private Frame? _frame;

    public ISettingsService SettingsService { get; }

    public SettingsViewModel(ISettingsService settingsService)
    {
        SettingsService = settingsService;
    }

    [RelayCommand]
    public void InitializeNavigation(Frame frame)
    {
        _frame = frame;
    }

    [RelayCommand]
    private void NavSelectionChanged(NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is SettingsNavigationItem navItem)
        {
            switch (navItem.Tag)
            {
                case "SettingsNavViewItem":
                    _frame?.Navigate(typeof(AppSettingsPage), null, args.RecommendedNavigationTransitionInfo);
                    return;
            }
        }
        else if (args.SelectedItem is Rule rule)
        {
            _frame?.Navigate(typeof(RuleSettingsPage), rule, args.RecommendedNavigationTransitionInfo);
        }
    }
}
