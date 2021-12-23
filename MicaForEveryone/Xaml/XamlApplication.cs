using System;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

namespace MicaForEveryone.Xaml
{
    public class XamlApplication : Application
    {
        private WindowsXamlManager _xamlManager;

        public XamlApplication()
        {
            _xamlManager = WindowsXamlManager.InitializeForCurrentThread();
            BeforeTranslateMessage += XamlApplication_BeforeTranslateMessage;
        }

        private void XamlApplication_BeforeTranslateMessage(Win32.Window window, ref MSG message, ref bool processed)
        {
            if (window is XamlWindow xamlWindow)
            {
                processed = xamlWindow.Interop.PreTranslateMessage(ref message);
            }
        }
    }
}
