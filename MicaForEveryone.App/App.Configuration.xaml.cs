using CommunityToolkit.Extensions.DependencyInjection;
using MicaForEveryone.App.Services;
using MicaForEveryone.App.ViewModels;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using System;

namespace MicaForEveryone.App;

public partial class App
{
    private IServiceProvider? _services;
    public static IServiceProvider Services
    {
        get
        {
            App currentApp = (App)Current;
            return currentApp._services ??= ConfigureServices();
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        ServiceCollection collection = new();

        collection.AddSingleton<IDispatchingService>(new DispatchingService(DispatcherQueue.GetForCurrentThread()));

        // Check if we are really running packaged.
        collection.AddSingleton<IVersionInfoService, PackagedVersionInfoService>();
        collection.AddSingleton<ISettingsService, PackagedSettingsService>();

        ConfigureServices(collection);

        return collection.BuildServiceProvider();
    }

    [Singleton(typeof(MainAppService))]
    [Singleton(typeof(RuleService), typeof(IRuleService))]
    [Transient(typeof(TrayIconViewModel))]
    [Transient(typeof(SettingsViewModel))]
    [Transient(typeof(AppSettingsPageViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}
