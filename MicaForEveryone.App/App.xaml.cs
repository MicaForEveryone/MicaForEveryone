using MicaForEveryone.App.Services;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        new JoinableTaskFactory(new JoinableTaskContext()).Run(async () =>
        {
            await Services.GetRequiredService<ISettingsService>().InitializeAsync();
            Services.GetRequiredService<MainAppService>().Initialize();
            _ = Services.GetRequiredService<IRuleService>().ApplyRulesToAllWindows();
        });
    }
}
