using System;
using Vanara.InteropServices;
using Vanara.PInvoke;

using MicaForEveryone.Rules;
using MicaForEveryone.Win32;

using static Vanara.PInvoke.DwmApi;

namespace MicaForEveryone.Extensions
{

    public static class DwmExtensions
    {
        private const DWMWINDOWATTRIBUTE DWMWA_MICA = (DWMWINDOWATTRIBUTE) 1029;
        private const DWMWINDOWATTRIBUTE DWMWA_IMMERSIVE_DARK_MODE = (DWMWINDOWATTRIBUTE) 20;
        private const DWMWINDOWATTRIBUTE DWMWA_SYSTEMBACKDROP_TYPE = (DWMWINDOWATTRIBUTE) 38;

        public static HRESULT SetMica(this HWND windowHandle, bool state)
        {
            var value = state ? 1 : 0;
            using var pinned = new PinnedObject(value);
            return DwmSetWindowAttribute(windowHandle, DWMWA_MICA, pinned, sizeof(int));
        }

        public static HRESULT SetBackdropType(this HWND windowHandle, DWM_SYSTEMBACKDROP_TYPE backdropType)
        {
            using var pinned = new PinnedObject((int)backdropType);
            return DwmSetWindowAttribute(windowHandle, DWMWA_SYSTEMBACKDROP_TYPE, pinned, sizeof(int));
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
