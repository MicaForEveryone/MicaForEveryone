using System;

using MicaForEveryone.Models;

namespace MicaForEveryone.Win32
{
    public enum DWM_SYSTEMBACKDROP_TYPE
    {
        DWMSBT_AUTO = 0,
        DWMSBT_NONE = 1,
        DWMSBT_MAINWINDOW = 2, // Mica
        DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
        DWMSBT_TABBEDWINDOW = 4 // Tabbed
    }

    public static class SystemBackdrop
    {
        public static bool IsSupported => Environment.OSVersion.Version.Build >= 22523;

        public static DWM_SYSTEMBACKDROP_TYPE ToDwmSystemBackdropType(this BackdropType rule) =>
            (DWM_SYSTEMBACKDROP_TYPE) rule;
    }
}
