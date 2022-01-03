using System;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Services;
using MicaForEveryone.Xaml;
using MicaForEveryone.Config;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        private readonly UI.App _uwpApp = new();

        public IServiceProvider Container { get; private set; }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException += UwpApp_UnhandledException;

            Container = RegiserServices();

            if (Environment.OSVersion.Version.Build < 22000)
            {
                var dialogService = Container.GetService<IDialogService>();
                dialogService.RunErrorDialog("Unsupported Windows Version", "This app requires at least Windows 11 (10.0.22000.0) to work.", 400, 275);
                return;
            }


            var mainWindow = Container.GetService<IViewService>().MainWindow;
            mainWindow.Activate();
            Run(mainWindow);

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public override void Dispose()
        {
            _uwpApp.Dispose();
            base.Dispose();
        }

        private IServiceProvider RegiserServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfigSource, ConfigFile>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IEventHookService, EventHookService>();
            services.AddSingleton<IRuleService, RuleHandler>();
            services.AddTransient<IViewModel, ViewModel>();
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<IDialogService, DialogService>();

            return services.BuildServiceProvider();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var dialogService = Container.GetService<IDialogService>();
            if (args.ExceptionObject is ParserError error)
            {
                dialogService.RunErrorDialog("Error in config file", error.Message, 576, 320);
            }
            else
            {
                dialogService.RunErrorDialog("Unhanlded Exception", args.ExceptionObject.ToString(), 576, 400);
            }
        }

        private void UwpApp_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            var dialogService = Container.GetService<IDialogService>();
            dialogService.RunErrorDialog("Unhanlded Exception in UI", args.Message, 576, 400);
        }
    }
}
