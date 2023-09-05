using MicaForEveryone.App.Dispatching;
using MicaForEveryone.App.Service;
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
    private static DispatcherQueue _dispatcherQueue;

    [STAThread]
    public static async Task Main(string[] _)
    {
        ComWrappersSupport.InitializeComWrappers();

        bool isRedirect = await DecideRedirection();
        if (!isRedirect)
        {
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                var context = new Dispatching.DispatcherQueueSynchronizationContext(
                    _dispatcherQueue = DispatcherQueue.GetForCurrentThread());
                SynchronizationContext.SetSynchronizationContext(context);
                new App();
            });
        }
    }

    private static async Task<bool> DecideRedirection()
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

    private static async void OnActivated(object? _, AppActivationArguments __)
    {
        await _dispatcherQueue.AsValueTask();
        App.Services.GetRequiredService<MainAppService>().ActivateSettings();
    }
}
