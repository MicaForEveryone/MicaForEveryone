﻿using MicaForEveryone.App.Services;
using MicaForEveryone.CoreUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Threading;
using System.Threading.Tasks;
using WinRT;

namespace MicaForEveryone.App;

class Program
{
    [STAThread]
    public static async Task Main(string[] _)
    {
        ComWrappersSupport.InitializeComWrappers();

        bool isRedirect = await DecideRedirectionAsync();
        if (!isRedirect)
        {

            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new Dispatching.DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
    }

    private static async Task<bool> DecideRedirectionAsync()
    {
        bool isRedirect = false;
        AppActivationArguments args = AppInstance.GetCurrent().GetActivatedEventArgs();
        AppInstance keyInstance = AppInstance.FindOrRegisterForKey("MicaForEveryone");

        if (keyInstance.IsCurrent)
        {
            keyInstance.Activated += OnActivated;
        }
        else
        {
            isRedirect = true;
            await keyInstance.RedirectActivationToAsync(args);
        }
        return isRedirect;
    }

#pragma warning disable VSTHRD100 // Avoid async void methods
    private static async void OnActivated(object? _, AppActivationArguments __)
#pragma warning restore VSTHRD100 // Avoid async void methods
    {
        await App.Services.GetRequiredService<IDispatchingService>().YieldAsync();
        App.Services.GetRequiredService<MainAppService>().ActivateSettings();
    }
}