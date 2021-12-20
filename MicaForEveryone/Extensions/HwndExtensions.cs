using System;
using System.Diagnostics;
using System.Text;
using Vanara.PInvoke;

using MicaForEveryone.Win32;
using MicaForEveryone.Models;

namespace MicaForEveryone.Extensions
{
    public static class HwndExtensions
    {
        public static string GetText(this HWND hwnd)
        {
            var length = User32.GetWindowTextLength(hwnd) + 1;
            var buffer = new StringBuilder(length);
            User32.GetWindowText(hwnd, buffer, length);
            return buffer.ToString();
        }

        public static string GetProcessName(this HWND hwnd)
        {
            User32.GetWindowThreadProcessId(hwnd, out var pid);
            return Process.GetProcessById((int) pid).ProcessName;
        }

        public static string GetClassName(this HWND hwnd)
        {
            var buffer = new StringBuilder(256);
            User32.GetClassName(hwnd, buffer, 256);
            return buffer.ToString();
        }

        public static bool HasCaption(this HWND hwnd)
        {
            var info = new User32.WINDOWINFO();
            User32.GetWindowInfo(hwnd, ref info);
            return info.dwStyle.HasFlag(User32.WindowStyles.WS_CAPTION);
        }

        public static void ApplyBackdropRule(this HWND windowHandle, BackdropType type)
        {
            if (SystemBackdrop.IsSupported)
            {
                if (type == BackdropType.Default)
                    return;
                
                windowHandle.SetBackdropType(type.ToDwmSystemBackdropType());
                return;
            }

            switch (type)
            {
                case BackdropType.Default:
                    return;
                case BackdropType.None:
                    windowHandle.SetMica(false);
                    return;
                case BackdropType.Mica:
                    windowHandle.SetMica(true);
                    return;
                default:
                    throw new PlatformNotSupportedException("Using `Acrylic` or `Tabbed` backdrop is not supported.");
            }
        }

        public static void SetWindowPos(this HWND windowHandle, HWND insertAfter, RECT rect, User32.SetWindowPosFlags flags)
        {
            if (!User32.SetWindowPos(windowHandle, insertAfter, rect.X, rect.Y, rect.Width, rect.Height, flags))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }
    }
}
