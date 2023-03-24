using System;
using Windows.ApplicationModel.Resources;
using Microsoft.Extensions.DependencyInjection;
using XclParser; 

using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Xaml;

namespace MicaForEveryone
{
    internal class App : XamlApplication
    {
        private readonly UI.App _uwpApp = new();

        public void RegisterExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException += UwpApp_UnhandledException;
        }

        public void UnregisterExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public override void Dispose()
        {
            _uwpApp.Dispose();
            base.Dispose();
        }
        
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var dialogService = Program.Container.GetService<IDialogService>();
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
            var dialogService = Program.Container.GetService<IDialogService>();

            var resources = ResourceLoader.GetForCurrentView();
            var header = resources.GetString("UnhandledUIException/Header");
            dialogService.RunErrorDialog(header, args.Message, 576, 400);
        }
    }
}
