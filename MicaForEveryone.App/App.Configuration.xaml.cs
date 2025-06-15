using CommunityToolkit.Extensions.DependencyInjection;
using MicaForEveryone.App.Services;
using MicaForEveryone.App.ViewModels;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.Storage;
using System;

#if !DEBUG
using Microsoft.Windows.ApplicationModel.Resources;
using System.Net;
using System.Net.NetworkInformation;
#endif

namespace MicaForEveryone.App;

public partial class App
{
    private static IServiceProvider? _services;
    public static IServiceProvider Services
    {
        get
        {
            return _services ??= ConfigureServices();
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection collection = new();

        collection.AddSingleton<IDispatchingService>(new DispatchingService(DispatcherQueue.GetForCurrentThread()));
        collection.AddSingleton<ILocalizationService>(new LocalizationService());

        // Check if we are really running packaged.
        collection.AddSingleton<IVersionInfoService, PackagedVersionInfoService>();
        collection.AddSingleton<ISettingsService, PackagedSettingsService>();
        collection.AddSingleton<IStartupService, PackagedStartupService>();
        collection.AddSingleton<IUpdateCheckerService, PackagedUpdateCheckerService>();

        string installId;
        if (ApplicationData.GetDefault().LocalSettings.Values.TryGetValue("AppCenterId", out object? rawId) && rawId is string parsedId)
        {
            installId = parsedId;
        }
        else
        {
            ApplicationData.GetDefault().LocalSettings.Values["AppCenterId"] = installId = Guid.NewGuid().ToString();
        }

#if !DEBUG
        if (DateTime.UtcNow < new DateTime(2026, 06, 30))
        {
            string appCenterString = new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "appsettings").GetString("AppCenterConnectionString");
            if (!string.IsNullOrEmpty(appCenterString))
            {
                collection.AddSingleton<ILoggingService, AppCenterLoggingService>(provider =>
                {
                    return new AppCenterLoggingService(appCenterString, installId, provider.GetRequiredService<IVersionInfoService>(), provider.GetRequiredService<ISettingsService>());
                });
            }
        }
        else
        {
            string appInsightsString = new ResourceLoader(ResourceLoader.GetDefaultResourceFilePath(), "appsettings").GetString("AppInsightsConnectionString");
            if (!string.IsNullOrEmpty(appInsightsString))
            {
                string domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;
                string hostName = Dns.GetHostName();

                if (!hostName.EndsWith(domainName, StringComparison.OrdinalIgnoreCase))
                {
                    hostName = $"{hostName}.{domainName}";
                }

                collection.AddSingleton<ILoggingService, AppInsightsLoggingService>(provider => new AppInsightsLoggingService(appInsightsString, new AppInsightsLoggingService.AppInsightsTags() { RoleInstance = hostName }, provider.GetRequiredService<ISettingsService>()));
            }
        }
#endif

        ConfigureServices(collection);

        return collection.BuildServiceProvider();
    }

    [Singleton(typeof(MainAppService))]
    [Singleton(typeof(RuleService), [typeof(IRuleService)])]
    [Singleton(typeof(ThemingService), [typeof(IThemingService)])]
    [Transient(typeof(SettingsViewModel))]
    [Transient(typeof(TrayIconViewModel))]
    [Transient(typeof(AddClassRuleContentDialogViewModel))]
    [Transient(typeof(AddProcessRuleContentDialogViewModel))]
    [Transient(typeof(AppSettingsPageViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}
