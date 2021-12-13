using Vanara.InteropServices;
using Vanara.PInvoke;

namespace MicaForEveryone.Extensions
{
    public static class DwmExtensions
    {
        private static DwmApi.DWMWINDOWATTRIBUTE DWMWA_MICA = (DwmApi.DWMWINDOWATTRIBUTE) 1029;
        private static DwmApi.DWMWINDOWATTRIBUTE DWMWA_IMMERSIVE_DARK_MODE = (DwmApi.DWMWINDOWATTRIBUTE) 20;

        public static HRESULT SetMica(this HWND hwnd, bool state)
        {
            var value = state ? 1 : 0;
            using var pinned = new PinnedObject(value);
            return DwmApi.DwmSetWindowAttribute(hwnd, DWMWA_MICA, pinned, sizeof(int));
        }

        public static HRESULT SetImmersiveDarkMode(this HWND hwnd, bool state)
        {
            var value = state ? 1 : 0;
            using var pinned = new PinnedObject(value);
            return DwmApi.DwmSetWindowAttribute(hwnd, DWMWA_IMMERSIVE_DARK_MODE, pinned, sizeof(int));
        }

        public static HRESULT ExtendFrameIntoClientArea(this HWND hwnd)
        {
            var margins = new DwmApi.MARGINS(-1);
            return DwmApi.DwmExtendFrameIntoClientArea(hwnd, margins);
        }
    }
}
