using System;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Services;
using MicaForEveryone.Xaml;
using MicaForEveryone.Config;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Views;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        private readonly UI.App _uwpApp = new();

        public IServiceProvider Container { get; private set; }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Container = RegiserServices();
            _uwpApp.Container = Container;

            if (Environment.OSVersion.Version.Build < 22000)
            {
                var dialogService = Container.GetService<IDialogService>();
                dialogService.RunErrorDialog("Unsupported Windows Version", "This app requires at least Windows 11 (10.0.22000.0) to work.", 400, 275);
                return;
            }

            _uwpApp.UnhandledException += UwpApp_UnhandledException;

            var mainWindow = new MainWindow();
            var viewService = Container.GetService<IViewService>();
            viewService.Run(mainWindow);

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        private IServiceProvider RegiserServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IConfigSource, ConfigFile>();
            services.AddSingleton<IConfigService, ConfigService>();
            services.AddSingleton<IEventHookService, EventHookService>();
            services.AddSingleton<IRuleService, RuleHandler>();
            services.AddSingleton<IDialogService, DialogService>();

            services.AddTransient<ITrayIconViewModel, TrayIconViewModel>();
            services.AddTransient<IContentDialogViewModel, ContentDialogViewModel>();

            services.AddSingleton<IViewService, ViewService>();

            return services.BuildServiceProvider();
        }

        public override void Dispose()
        {
            _uwpApp.Dispose();
            base.Dispose();
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
                dialogService.RunErrorDialog("Unhandled Exception", args.ExceptionObject.ToString(), 576, 400);
            }
        }

        private void UwpApp_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            var dialogService = Container.GetService<IDialogService>();
            dialogService.RunErrorDialog("Unhanlded Exception in UI", args.Message, 576, 400);
        }
    }
}
