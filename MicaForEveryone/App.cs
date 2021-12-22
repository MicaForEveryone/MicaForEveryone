using System;

using MicaForEveryone.Xaml;

namespace MicaForEveryone
{
    internal partial class App : XamlApplication, IDisposable
    {
        private readonly UWP.App _uwpApp = new();

        public void Run()
        {
            if (Environment.OSVersion.Version.Build < 22000)
            {
                ShowWindows11RequiredDialog();
                return;
            }

            InitializeMainWindow();
            InitializeViewModel();
            InitializeRuleHandler();
            InitializeEventHook();
            UpdateViewModel();
            
            Run(_mainWindow);
        }

        public void Dispose()
        {
            _eventHook.Dispose();
            _mainWindow.Dispose();
            _uwpApp.Dispose();
        }
    }
}
