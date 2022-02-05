using MicaForEveryone.Win32.PInvoke;
using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public static class SystemMetrics
    {
        static SystemMetrics()
        {
            Refresh();
        }

        public static int CaptionHeight { get; private set; }

        public static int FrameWidth { get; private set; }

        public static int FrameHeight { get; private set; }

        public static void Refresh()
        {
            CaptionHeight = GetSystemMetrics(SystemMetric.SM_CYCAPTION);
            FrameWidth = GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);
            FrameHeight = GetSystemMetrics(SystemMetric.SM_CXSIZEFRAME);
        }
    }
}
