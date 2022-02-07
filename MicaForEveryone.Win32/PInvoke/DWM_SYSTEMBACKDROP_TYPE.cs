namespace MicaForEveryone.Win32.PInvoke
{
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
}
