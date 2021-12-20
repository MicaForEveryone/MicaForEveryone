using System;
using Vanara.PInvoke;
using Windows.UI.Xaml.Hosting;

namespace MicaForEveryone.Xaml
{
    public class XamlApplication : Application
    {
        private WindowsXamlManager _xamlManager;

        public XamlApplication()
        {
            BeforeRun += XamlApplication_BeforeRun;
            BeforeTranslateMessage += XamlApplication_BeforeTranslateMessage;
        }

        private void XamlApplication_BeforeRun(object sender, EventArgs e)
        {
            _xamlManager = WindowsXamlManager.InitializeForCurrentThread();
        }

        private void XamlApplication_BeforeTranslateMessage(Win32.Window window, ref MSG message, ref bool processed)
        {
            if (window is XamlWindow xamlWindow)
            {
                processed = xamlWindow.GetXamlWindowInterop().PreTranslateMessage(ref message);
            }
        }
    }
}
