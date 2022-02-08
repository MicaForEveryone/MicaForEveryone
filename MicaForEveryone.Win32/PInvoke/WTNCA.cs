using System;

namespace MicaForEveryone.Win32.PInvoke
{
    [Flags]
    public enum WTNCA
    {
        /// <summary>Prevents the window caption from being drawn.</summary>
        WTNCA_NODRAWCAPTION = 0x00000001,

        /// <summary>Prevents the system icon from being drawn.</summary>
        WTNCA_NODRAWICON = 0x00000002,

        /// <summary>Prevents the system icon menu from appearing.</summary>
        WTNCA_NOSYSMENU = 0x00000004,

        /// <summary>Prevents mirroring of the question mark, even in right-to-left (RTL) layout.</summary>
        WTNCA_NOMIRRORHELP = 0x00000008
    }
}
