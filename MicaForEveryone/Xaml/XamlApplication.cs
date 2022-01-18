using System;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

using MicaForEveryone.Win32;

namespace MicaForEveryone.Xaml
{
    public class XamlApplication : Application, IDisposable
    {
        private WindowsXamlManager _xamlManager;

        public XamlApplication()
        {
            _xamlManager = WindowsXamlManager.InitializeForCurrentThread();
            BeforeTranslateMessage += XamlApplication_BeforeTranslateMessage;
        }

        public virtual void Dispose()
        {
            _xamlManager.Dispose();
        }

        private void XamlApplication_BeforeTranslateMessage(NativeWindow window, ref MSG message, ref bool processed)
        {
            if (window is XamlWindow xamlWindow)
            {
                processed = xamlWindow.Interop.PreTranslateMessage(ref message);
            }
        }
    }
}
