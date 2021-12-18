using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;
using Microsoft.Toolkit.Win32.UI.XamlHost;

namespace MicaForEveryone.Xaml
{
    public class XamlWindow : Win32.Window
    {
        private DesktopWindowXamlSource _xamlSource;

        public XamlWindow(UIElement view)
        {
            View = view;
            Activated += XamlWindow_Activated;
        }

        public UIElement View { get; protected set; }

        private void XamlWindow_Activated(object sender, EventArgs args)
        {
            Show();

            _xamlSource = new()
            {
                Content = View,
            };
        }

        public IDesktopWindowXamlSourceNative2 GetInterop() => _xamlSource.GetInterop<IDesktopWindowXamlSourceNative2>();
    }
}
