using CommunityToolkit.Extensions.DependencyInjection;
using MicaForEveryone.App.Services;
using MicaForEveryone.App.ViewModels;
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

        ConfigureServices(collection);

        return collection.BuildServiceProvider();
    }


    [Singleton(typeof(MainAppService))]
    [Singleton(typeof(TrayIconViewModel))]
    [Transient(typeof(SettingsViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}
