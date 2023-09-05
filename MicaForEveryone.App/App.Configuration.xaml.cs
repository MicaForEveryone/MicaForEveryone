using CommunityToolkit.Extensions.DependencyInjection;
using MicaForEveryone.App.Service;
using MicaForEveryone.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
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
            if (currentApp._services == null)
            {
                ServiceCollection collection = new ServiceCollection();
                ConfigureServices(collection);
                currentApp._services = collection.BuildServiceProvider();
            }
            return currentApp._services;
        }
    }


    [Singleton(typeof(MainAppService))]
    [Singleton(typeof(TrayIconViewModel))]
    private static partial void ConfigureServices(IServiceCollection services);
}
