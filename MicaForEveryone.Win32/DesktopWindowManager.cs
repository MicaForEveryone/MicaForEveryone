using System;
using System.Runtime.InteropServices;

using MicaForEveryone.Models;
using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public static class DesktopWindowManager
    {
        private const uint DWMWA_MICA = 1029;
        private const uint DWMWA_IMMERSIVE_DARK_MODE = 20;
        private const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const uint DWMWA_CAPTION_COLOR = 35;
        private const uint DWMWA_TEXT_COLOR = 36;

        public static void EnableMicaIfSupported(IntPtr target)
        {
            if (Environment.OSVersion.Version.Build >= 22523)
            {
                SetBackdropType(target, BackdropType.Mica);
            }
            else if (Environment.OSVersion.Version.Build >= 22000)
            {
                SetMica(target, true);
            }
        }

        public static void SetMica(IntPtr target, bool state)
        {
            var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(target, DWMWA_MICA, value.AddrOfPinnedObject(), sizeof(int));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        // only supported on Windows 11 build 22523+
        public static void SetBackdropType(IntPtr target, DWM_SYSTEMBACKDROP_TYPE backdropType)
        {
            var value = GCHandle.Alloc(backdropType, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(target, DWMWA_SYSTEMBACKDROP_TYPE, value.AddrOfPinnedObject(), sizeof(DWM_SYSTEMBACKDROP_TYPE));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }

        }

        public static void SetBackdropType(IntPtr target, BackdropType backdropType)
        {
            SetBackdropType(target, (DWM_SYSTEMBACKDROP_TYPE)backdropType);
        }

        public static void SetImmersiveDarkMode(IntPtr target, bool state)
        {
            var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(target, DWMWA_IMMERSIVE_DARK_MODE, value.AddrOfPinnedObject(), sizeof(int));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        public static void ExtendFrameIntoClientArea(IntPtr target)
        {
            var margins = new MARGINS(-1);
            var result = DwmExtendFrameIntoClientArea(target, margins);
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        public static void SetCaptionColor(IntPtr target, COLORREF color)
        {
            var value = GCHandle.Alloc(color, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(target, DWMWA_CAPTION_COLOR, value.AddrOfPinnedObject(), Marshal.SizeOf(color));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        public static void SetCaptionTextColor(IntPtr target, COLORREF color)
        {
            var value = GCHandle.Alloc(color, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(target, DWMWA_TEXT_COLOR, value.AddrOfPinnedObject(), Marshal.SizeOf(color));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        public static void EnableBlurBehind(IntPtr target)
        {
            var value = new DWM_BLURBEHIND(true);
            var result = DwmEnableBlurBehindWindow(target, value);
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }
    }
}
