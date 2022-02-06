using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Windows.ApplicationModel.Resources;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Services;
using MicaForEveryone.ViewModels;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Xaml;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        private readonly UI.App _uwpApp = new();

        public IServiceProvider Container { get; private set; }

        public void Run()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Container = RegisterServices();
            _uwpApp.Container = Container;

            if (Environment.OSVersion.Version.Build < 22000)
            {
                var dialogService = Container.GetService<IDialogService>();

                var resources = ResourceLoader.GetForCurrentView();
                var header = resources.GetString("UnsupportedError/Header");
                var message = resources.GetString("UnsupportedError/Message");
                dialogService.RunErrorDialog(header, message, 400, 275);

                return;
            }

            _uwpApp.UnhandledException += UwpApp_UnhandledException;

            var viewService = Container.GetService<IViewService>();
            viewService.Run();

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public override void Dispose()
        {
            _uwpApp.Dispose();
            base.Dispose();
        }

        private string GetConfigFilePath()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                return args[1];
            }
            
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var configPath = Path.Join(appData, "Mica For Everyone", "MicaForEveryone.conf");

            if (!File.Exists(configPath))
            {
                var appFolder = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var defaultConfigPath = Path.Join(appFolder, "MicaForEveryone.conf");
                if (File.Exists(defaultConfigPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                    File.Copy(defaultConfigPath, configPath);
                }
            }

            return configPath;
        }

        private IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            var configSource = new ConfigFile(GetConfigFilePath());
            var configService = new ConfigService(configSource);
            services.AddSingleton<IConfigService>(configService);

            services.AddSingleton<IRuleService, RuleService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IViewService, ViewService>();

            services.AddTransient<ITrayIconViewModel, TrayIconViewModel>();
            services.AddTransient<IContentDialogViewModel, ContentDialogViewModel>();
            services.AddTransient<ISettingsViewModel, SettingsViewModel>();
            services.AddTransient<IGeneralSettingsViewModel, GeneralSettingsViewModel>();
            services.AddTransient<IRuleSettingsViewModel, RuleSettingsViewModel>();
            services.AddTransient<IAddProcessRuleViewModel, AddProcessRuleViewModel>();
            services.AddTransient<IAddClassRuleViewModel, AddClassRuleViewModel>();

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
