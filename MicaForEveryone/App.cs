using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Services;
using MicaForEveryone.Xaml;
using MicaForEveryone.Config;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Views;
using MicaForEveryone.Models;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        public UI.App UwpApp { get; } = new();

        public IServiceProvider Container { get; private set; }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Container = RegisterServices();
            UwpApp.Container = Container;

            if (Environment.OSVersion.Version.Build < 22000)
            {
                var dialogService = Container.GetService<IDialogService>();
                dialogService.RunErrorDialog("Unsupported Windows Version", "This app requires at least Windows 11 (10.0.22000.0) to work.", 400, 275);
                return;
            }

            UwpApp.UnhandledException += UwpApp_UnhandledException;

            var viewService = Container.GetService<IViewService>();
            var ruleService = Container.GetService<IRuleService>();
            var configService = Container.GetService<IConfigService>();
            var eventHookService = Container.GetService<IEventHookService>();

            configService.LoadAsync().Wait();
            ruleService.SystemTitlebarColorMode = viewService.SystemColorMode;
            eventHookService.Start();
            ruleService.MatchAndApplyRuleToAllWindows();

            using var mainWindow = new MainWindow();
            viewService.Run(mainWindow);

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            UwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public override void Dispose()
        {
            UwpApp.Dispose();
            base.Dispose();
        }

        private string GetConfigFilePath()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                return args[1];
            }
            if (File.Exists("MicaForEveryone.conf"))
            {
                return "MicaForEveryone.conf";
            }
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Join(appData, "Mica For Everyone", "MicaForEveryone.conf");
        }

        private IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            var configSource = new ConfigFile(GetConfigFilePath());
            var configService = new ConfigService(configSource);
            services.AddSingleton<IConfigService>(configService);

            services.AddSingleton<IEventHookService, EventHookService>();
            services.AddSingleton<IRuleService, RuleHandler>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IViewService, ViewService>();

            services.AddTransient<ITrayIconViewModel, TrayIconViewModel>();
            services.AddTransient<IContentDialogViewModel, ContentDialogViewModel>();

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
                dialogService.RunErrorDialog("Unhandled Exception", args.ExceptionObject.ToString(), 576, 400);
            }
        }

        private void UwpApp_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            var dialogService = Container.GetService<IDialogService>();
            dialogService.RunErrorDialog("Unhandled Exception in UI", args.Message, 576, 400);
        }
    }
}
