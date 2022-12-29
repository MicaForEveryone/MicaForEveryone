using System;
using System.Threading;
using Windows.ApplicationModel.Resources;
using Microsoft.Extensions.DependencyInjection;
using XclParser; 

using MicaForEveryone.Interfaces;
using MicaForEveryone.Services;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Xaml;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        private readonly UI.App _uwpApp = new();

        private Mutex _siMutex = new(true, "Mica For Everyone");

        public IServiceProvider Container { get; private set; }

        public App()
        {
            Container = RegisterServices();
        }

        public bool IsItFirstInstance()
        {
            return _siMutex.WaitOne(0, true);
        }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException += UwpApp_UnhandledException;

            // load settings before using view service
            var srvSettings = Container.GetService<ISettingsService>();
            srvSettings.Load();

            var srvView = Container.GetService<IViewService>();
            srvView.Run();

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public override void Dispose()
        {
            _uwpApp.Dispose();
            base.Dispose();
        }

        private IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<ISettingsContainer>(container =>
                IsPackaged ? new UwpSettingsContainer()
                : new RegistrySettingsContainer());
            services.AddTransient<IConfigParser, XclParserService>();
            services.AddTransient<IConfigFile>(container =>
                IsPackaged ? new UwpConfigFile(container.GetService<IConfigParser>())
                : new Win32ConfigFile(container.GetService<IConfigParser>()));
            services.AddTransient<ITrayIconViewModel, TrayIconViewModel>();
            services.AddTransient<UI.ViewModels.IContentDialogViewModel, ContentDialogViewModel>();
            services.AddTransient<ISettingsViewModel, SettingsViewModel>();
            services.AddTransient<IGeneralSettingsViewModel, GeneralSettingsViewModel>();
            services.AddTransient<IRuleSettingsViewModel, RuleSettingsViewModel>();
            services.AddTransient<IAddProcessRuleViewModel, AddProcessRuleViewModel>();
            services.AddTransient<IAddClassRuleViewModel, AddClassRuleViewModel>();

            services.AddSingleton<IStartupService>(container =>
                IsPackaged ? new UwpStartupService()
                : new Win32StartupService());
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IRuleService, RuleService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<ITaskSchedulerService, TaskSchedulerService>();
            services.AddSingleton<ILogger, Logger>();
            services.AddTransient<ILogsViewModel, LogsViewModel>();

            return services.BuildServiceProvider();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var dialogService = Container.GetService<IDialogService>();
            var resources = ResourceLoader.GetForCurrentView();
            if (args.ExceptionObject is ParserError error)
            {
                var header = resources.GetString("ConfigFileError/Header");
                dialogService.RunErrorDialog(header, error.Message, 576, 320);
            }
            else
            {
                var header = resources.GetString("UnhandledException/Header");
                dialogService.RunErrorDialog(header, args.ExceptionObject.ToString(), 576, 400);
            }
        }

        private void UwpApp_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            var dialogService = Container.GetService<IDialogService>();

            var resources = ResourceLoader.GetForCurrentView();
            var header = resources.GetString("UnhandledUIException/Header");
            dialogService.RunErrorDialog(header, args.Message, 576, 400);
        }
    }
}
