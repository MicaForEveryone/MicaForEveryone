using System;
using Vanara.InteropServices;
using Vanara.PInvoke;

using static Vanara.PInvoke.DwmApi;

namespace MicaForEveryone.Extensions
{
    public static class DwmExtensions
    {
        private const DWMWINDOWATTRIBUTE DWMWA_MICA = (DWMWINDOWATTRIBUTE) 1029;
        private const DWMWINDOWATTRIBUTE DWMWA_IMMERSIVE_DARK_MODE = (DWMWINDOWATTRIBUTE) 20;
        private const DWMWINDOWATTRIBUTE DWMWA_SYSTEMBACKDROP_TYPE = (DWMWINDOWATTRIBUTE) 38;

        enum DWM_SYSTEMBACKDROP_TYPE
        {
            DWMSBT_DISABLED = 1,
            DWMSBT_MAINWINDOW = 2, // Mica
            DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
            DWMSBT_TABBEDWINDOW = 4 // Tabbed
        }

        public static HRESULT SetMica(this HWND windowHandle, bool state)
        {
            if (Environment.OSVersion.Version.Build < 22523)
            {
                var value = state ? 1 : 0;
                using var pinned = new PinnedObject(value);
                return DwmSetWindowAttribute(windowHandle, DWMWA_MICA, pinned, sizeof(int));
            }
            else
            {
                // use new API available on Windows 11 build 22553
                var value = state ? 
                    DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW :
                    DWM_SYSTEMBACKDROP_TYPE.DWMSBT_DISABLED;
                using var pinned = new PinnedObject(value);
                return DwmSetWindowAttribute(windowHandle, DWMWA_SYSTEMBACKDROP_TYPE, pinned, sizeof(int));
            }
        }

        public static HRESULT SetImmersiveDarkMode(this HWND windowHandle, bool state)
        {
            var value = state ? 1 : 0;
            using var pinned = new PinnedObject(value);
            return DwmSetWindowAttribute(windowHandle, DWMWA_IMMERSIVE_DARK_MODE, pinned, sizeof(int));
        }

        public static HRESULT ExtendFrameIntoClientArea(this HWND windowHandle)
        {
            var margins = new MARGINS(-1);
            return DwmExtendFrameIntoClientArea(windowHandle, margins);
        }
    }
}
