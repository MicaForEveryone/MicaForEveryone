using MicaForEveryone.App.Services;
using MicaForEveryone.App.Views;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace MicaForEveryone.App
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            UnhandledException += OnUnhandledException;
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ISettingsService settingsService = Services.GetRequiredService<ISettingsService>();
            try
            {
                await settingsService.InitializeAsync();
            }
            catch (JsonException)
            {
                DialogWindow window = new("Invalid configuration file", "The configuration file is either malformed or corrupt.");
                ContentDialogResult result = await window.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    await settingsService.OpenConfigurationFileAsync();
                }
                window.Close();
                return;
            }
            Services.GetRequiredService<IRuleService>().Initialize();
            Services.GetRequiredService<MainAppService>().Initialize();
            _ = Services.GetRequiredService<IRuleService>().ApplyRulesToAllWindowsAsync();

            // MODIFICATION: Check version and show What's New dialog
            await CheckForUpdateAndShowWhatsNewAsync();
        }

        // MODIFICATION: New method to handle showing the What's New dialog
        private async Task CheckForUpdateAndShowWhatsNewAsync()
        {
            var versionInfo = Services.GetRequiredService<IVersionInfoService>();
            string currentVersion = versionInfo.GetVersion();
            
            var settings = ApplicationData.Current.LocalSettings;
            string? lastLaunchedVersion = settings.Values["LastLaunchedVersion"] as string;

            // Show dialog only if the version has changed
            if (lastLaunchedVersion == null || lastLaunchedVersion != currentVersion)
            {
                // We need a XamlRoot to show a ContentDialog.
                // The MainAppService creates a hidden window for the tray icon,
                // but our main SettingsWindow might not be open.
                // A small delay ensures the tray window is ready.
                await Task.Delay(100); 
                var mainAppService = Services.GetRequiredService<MainAppService>();
                var xamlRoot = mainAppService.GetMainWindowXamlRoot();
                if (xamlRoot != null)
                {
                    var dialog = new WhatsNewDialog(currentVersion);
                    dialog.XamlRoot = xamlRoot;
                    await dialog.ShowAsync();
                }
                
                settings.Values["LastLaunchedVersion"] = currentVersion;
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if !DEBUG
            ILoggingService? loggingService = App.Services.GetService<ILoggingService>();
            loggingService?.LogException(e.Exception);
            _ = loggingService?.FlushAsync();
#endif
        }
    }
}
