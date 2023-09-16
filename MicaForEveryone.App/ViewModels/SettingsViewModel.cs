using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.App.Views;
using MicaForEveryone.CoreUI;
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
    private void NavItemInvoked(NavigationViewItemInvokedEventArgs args)
    {
        switch (args.InvokedItemContainer.Tag)
        {
            case "SettingsNavViewItem":
                _frame?.Navigate(typeof(AppSettingsPage), null, args.RecommendedNavigationTransitionInfo);
                return;
        }
    }
}
