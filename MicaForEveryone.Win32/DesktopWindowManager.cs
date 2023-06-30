using System;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public static class DesktopWindowManager
    {
        private const uint DWMWA_MICA = 1029;
        private const uint DWMWA_IMMERSIVE_DARK_MODE = 20;
        private const uint DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const uint DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const uint DWMWA_CAPTION_COLOR = 35;
        private const uint DWMWA_TEXT_COLOR = 36;

        /// <summary>
        /// Check whether Windows build is 19041 or higher, that supports <see cref="SetImmersiveDarkMode(IntPtr, bool)"/>.
        /// </summary>
        public static bool IsImmersiveDarkModeSupported { get; } =
            Environment.OSVersion.Version.Build >= 19041;

        /// <summary>
        /// Check whether Windows build is 22000 or higher, that supports <see cref="SetMica(IntPtr, bool)"/>.
        /// </summary>
        public static bool IsUndocumentedMicaSupported { get; } =
            Environment.OSVersion.Version.Build >= 22000;

        /// <summary>
        /// Check whether Windows Windows build is 22523 or higher, that supports <see cref="SetBackdropType(IntPtr, DWM_SYSTEMBACKDROP_TYPE)"/>
        /// </summary>
        public static bool IsBackdropTypeSupported { get; } =
            Environment.OSVersion.Version.Build >= 22523;

        /// <summary>
        /// Check whether Windows Windows build is 22000 or higher, that supports <see cref="SetCornerPreference(IntPtr, DWM_WINDOW_CORNER_PREFERENCE)"/>
        /// </summary>
        public static bool IsCornerPreferenceSupported { get; } =
            Environment.OSVersion.Version.Build >= 22000;

        /// <summary>
        /// Enable Mica on target window with <see cref="SetMica(IntPtr, bool)"/> or <see cref="SetBackdropType(IntPtr, DWM_SYSTEMBACKDROP_TYPE)"/> if supported.
        /// </summary>
        public static void EnableMicaIfSupported(IntPtr hWnd)
        {
            if (IsBackdropTypeSupported)
            {
                SetBackdropType(hWnd, DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MAINWINDOW);
            }
            else if (IsUndocumentedMicaSupported)
            {
                SetMica(hWnd, true);
            }
        }

        /// <summary>
        /// Enable or Disable Mica on target window
        /// Supported on Windows builds from 22000 to 22523. It doesn't work on 22523, use <see cref="SetBackdropType(IntPtr, DWM_SYSTEMBACKDROP_TYPE)"/> instead.
        /// </summary>
        public static void SetMica(IntPtr hWnd, bool state)
        {
            var value = GCHandle.Alloc(state ? 1 : 0, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_MICA, value.AddrOfPinnedObject(), sizeof(int));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        /// <summary>
        /// Set backdrop type on target window
        /// Requires Windows build 22523 or higher.
        /// </summary>
        public static void SetBackdropType(IntPtr hWnd, DWM_SYSTEMBACKDROP_TYPE backdropType)
        {
            var value = GCHandle.Alloc((uint)backdropType, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_SYSTEMBACKDROP_TYPE, value.AddrOfPinnedObject(), sizeof(uint));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }

        }
        
        /// <summary>
        /// Set corner preference on target window
        /// Requires Windows build 22000 or higher.
        /// </summary>
        public static void SetCornerPreference(IntPtr hWnd, DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
        {
            var value = GCHandle.Alloc((uint)cornerPreference, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_WINDOW_CORNER_PREFERENCE, value.AddrOfPinnedObject(), sizeof(uint));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }

        }
        
        /// <summary>
        /// Enable or disable immersive dark mode.
        /// Requires Windows build 19041 or higher.
        /// </summary>
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

        /// <summary>
        /// Change background color of titlebar.
        /// Requires Windows build 22000 or higher.
        /// </summary>
        public static void SetCaptionColor(IntPtr hWnd, COLORREF color)
        {
            if (Environment.OSVersion.Version.Build < 22000)
                return;
            
            var value = GCHandle.Alloc(color, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_CAPTION_COLOR, value.AddrOfPinnedObject(), Marshal.SizeOf(color));
            value.Free();
            if (result != 0)
            {
                throw Marshal.GetExceptionForHR(result);
            }
        }

        /// <summary>
        /// Change text color of titlebar.
        /// Requires Windows build 22000 or higher.
        /// </summary>
        public static void SetCaptionTextColor(IntPtr hWnd, COLORREF color)
        {
            if (Environment.OSVersion.Version.Build < 22000)
                return;
            
            var value = GCHandle.Alloc(color, GCHandleType.Pinned);
            var result = DwmSetWindowAttribute(hWnd, DWMWA_TEXT_COLOR, value.AddrOfPinnedObject(), Marshal.SizeOf(color));
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

            var accentPolicy = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND | AccentState.ACCENT_ENABLE_GRADIENT,
                GradientColor = (152 << 24) | (0x2B2B2B & 0xFFFFFF),
            };
            var accentSize = Marshal.SizeOf(accentPolicy);
            var accentPolicyPtr = Marshal.AllocHGlobal(accentSize);
            Marshal.StructureToPtr(accentPolicy, accentPolicyPtr, false);
            var compositionAttributeData = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                Data = accentPolicyPtr,
            };
            compositionAttributeData.SizeOfData = Marshal.SizeOf(compositionAttributeData);
            SetWindowCompositionAttribute(target, ref compositionAttributeData);
            Marshal.FreeHGlobal(accentPolicyPtr);
        }
    }
}
