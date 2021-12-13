using System.Diagnostics;
using System.Text;
using Vanara.PInvoke;

namespace MicaForEveryone
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
    }
}
