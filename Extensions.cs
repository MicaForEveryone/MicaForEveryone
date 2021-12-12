using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;

namespace MicaForEveryone
{
    public static class Extensions
    {
        public static string GetWindowTitle(this HWND hwnd)
        {
            var length = User32.GetWindowTextLength(hwnd) + 1;
            var buffer = new StringBuilder(length);
            User32.GetWindowText(hwnd, buffer, length);
            return buffer.ToString();
        }

        public static string GetWindowProcessName(this HWND hwnd)
        {
            User32.GetWindowThreadProcessId(hwnd, out var pid);
            using var process = Kernel32.OpenProcess(ACCESS_MASK.GENERIC_READ, false, pid);
            var buffer = new StringBuilder(256);
            Kernel32.GetProcessImageFileName(process, buffer, 256);
            return buffer.ToString();
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
    }
}
