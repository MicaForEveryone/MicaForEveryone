using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.App.Views;
using Microsoft.UI.Xaml.Controls;

namespace MicaForEveryone.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private Frame? _frame;

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
                break;
        }
    }
}
