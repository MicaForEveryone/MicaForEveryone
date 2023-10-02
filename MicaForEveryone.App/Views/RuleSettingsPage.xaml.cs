using CommunityToolkit.Mvvm.Input;
using MicaForEveryone.CoreUI;
using MicaForEveryone.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RuleSettingsPage : Page
{
    private Rule? Rule { get; set; }
    private ISettingsService SettingsService { get; }

    public RuleSettingsPage()
    {
        this.InitializeComponent();

        SettingsService = App.Services.GetRequiredService<ISettingsService>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        Rule = Unsafe.As<Rule>(e.Parameter);
        base.OnNavigatedTo(e);
    }

    public static string GetTitleBarColorLocalized(TitleBarColorMode titleBarColorMode)
    {
        return App.Services.GetRequiredService<ILocalizationService>().GetLocalizedTitleBarColor(titleBarColorMode);
    }

    [RelayCommand]
    public async Task ComboBoxChangedAsync(SelectionChangedEventArgs args)
    {
        if (args.RemovedItems!.Count == 0)
        {
            return;
        }

        await SettingsService.SaveAsync();
    }
}
