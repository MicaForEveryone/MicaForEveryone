using MicaForEveryone.App.Services;
using MicaForEveryone.App.Views;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MicaForEveryone.App
{
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
            this.InitializeComponent();

            UnhandledException += OnUnhandledException;
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
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
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
#if !DEBUG
            ILoggingService? loggingService = App.Services.GetService<ILoggingService>();
            loggingService?.LogException(e.Exception);
            loggingService?.FlushAsync().Wait();
#endif
        }
    }
}
