namespace MicaForEveryone.Win32.PInvoke
{
    public enum DWM_SYSTEMBACKDROP_TYPE : uint
    {
        DWMSBT_AUTO = 0,
        DWMSBT_NONE = 1,
        DWMSBT_MAINWINDOW = 2, // Mica
        DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
        DWMSBT_TABBEDWINDOW = 4 // Tabbed
    }
}
