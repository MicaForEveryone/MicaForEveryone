namespace MicaForEveryone.Win32.PInvoke
{
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
}
