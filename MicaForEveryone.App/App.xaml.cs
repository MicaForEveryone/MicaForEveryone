// ... (existing using statements)
using MicaForEveryone.App.Views;
using Windows.Storage;

namespace MicaForEveryone.App
{
    public partial class App : Application
    {
        // ... (existing constructor)

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // ... (existing OnLaunched logic)
            
            Services.GetRequiredService<IRuleService>().Initialize();
            Services.GetRequiredService<MainAppService>().Initialize();
            _ = Services.GetRequiredService<IRuleService>().ApplyRulesToAllWindowsAsync();

            await CheckForUpdateAndShowWhatsNewAsync();
        }

        private async Task CheckForUpdateAndShowWhatsNewAsync()
        {
            var versionInfo = Services.GetRequiredService<IVersionInfoService>();
            string currentVersion = versionInfo.GetVersion();
            
            var settings = ApplicationData.Current.LocalSettings;
            string lastLaunchedVersion = settings.Values["LastLaunchedVersion"] as string ?? "0.0.0.0";

            if (currentVersion != lastLaunchedVersion)
            {
                var dialog = new WhatsNewDialog(currentVersion);
                dialog.XamlRoot = Services.GetRequiredService<MainAppService>().GetMainWindowXamlRoot(); // A bit of a hack to get the XamlRoot
                await dialog.ShowAsync();
                settings.Values["LastLaunchedVersion"] = currentVersion;
            }
        }
        
        // ... (existing UnhandledException handler)
    }
}
