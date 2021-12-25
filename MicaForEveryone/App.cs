using System;

using MicaForEveryone.Xaml;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        private readonly UI.App _uwpApp = new();

        public void Run()
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                ShowWindows11RequiredDialog();
                return;
            }

            InitializeMainWindow();
            InitializeViewModel();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException += UwpApp_UnhandledException; ;

            InitializeRuleHandler();
            InitializeEventHook();
            UpdateViewModel();
            
            Run(_mainWindow);

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public void Dispose()
        {
            _eventHook.Dispose();
            _mainWindow.Dispose();
            _uwpApp.Dispose();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            ShowUnhandledExceptionDialog(args.ExceptionObject as Exception ??
                new Exception(args.ExceptionObject.ToString()));
        }

        private void UwpApp_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            ShowUnhandledExceptionDialog(args.Exception);
        }
    }
}
