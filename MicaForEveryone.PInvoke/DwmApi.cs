using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Windowing;
using static MicaForEveryone.PInvoke.Generic;

namespace MicaForEveryone.PInvoke;

public unsafe static class DwmApi
{
    #region Type Declarations
    /// <summary>
    /// Type of system backdrop to be rendered by DWM
    /// </summary>
    public enum DWM_SYSTEMBACKDROP_TYPE : uint
    {
        DWMSBT_AUTO = 0,

        /// <summary>
        /// no backdrop
        /// </summary>
        DWMSBT_NONE = 1,

        /// <summary>
        /// Use tinted blurred wallpaper backdrop (Mica)
        /// </summary>
        DWMSBT_MAINWINDOW = 2,

        /// <summary>
        /// Use Acrylic backdrop
        /// </summary>
        DWMSBT_TRANSIENTWINDOW = 3,

        /// <summary>
        /// Use blurred wallpaper backdrop
        /// </summary>
        DWMSBT_TABBEDWINDOW = 4
    }

    /// <summary>
    /// Type of corner preference to be rendered by DWM
    /// </summary>
    public enum DWM_WINDOW_CORNER_PREFERENCE : uint
    {
        /// <summary>
        /// let the system decide when to round window corners
        /// </summary>
        DWMWCP_DEFAULT = 0,

        /// <summary>
        /// never round window corners
        /// </summary>
        DWMWCP_DONOTROUND = 1,

        /// <summary>
        /// round the corners, if appropriate
        /// </summary>
        DWMWCP_ROUND = 2,

        /// <summary>
        /// round the corners if appropriate, with a small radius
        /// </summary>
        DWMWCP_ROUNDSMALL = 3
    }
    #endregion

    #region Properties
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
    #endregion

    #region Methods
    [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute", ExactSpelling = true)]
    public static extern HRESULT DwmSetWindowAttribute(HWND hwnd, uint dwAttribute, void* pvAttribute, uint cbAttribute);
    #endregion
}
