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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException += UwpApp_UnhandledException;

            InitializeRuleHandler();

            InitializeMainWindow();
            InitializeViewModel();

            InitializeEventHook();
            UpdateViewModel();
            
            Run(_mainWindow);

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            _uwpApp.UnhandledException -= UwpApp_UnhandledException;
        }

        public override void Dispose()
        {
            _eventHook.Dispose();
            _mainWindow.Dispose();
            _uwpApp.Dispose();
            base.Dispose();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _eventHook.Unhook();
            ShowUnhandledExceptionDialog(args.ExceptionObject as Exception ??
                new Exception(args.ExceptionObject.ToString()));
            _mainWindow.Dispose();
        }

        private void UwpApp_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs args)
        {
            _eventHook.Unhook();
            ShowUnhandledExceptionDialog(args.Exception);
            _mainWindow.Dispose();
        }
    }
}
