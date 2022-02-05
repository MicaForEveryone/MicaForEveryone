namespace MicaForEveryone.Win32.PInvoke
{
    public enum NIM
    {
        /// <summary>
        /// Adds an icon to the status area. The icon is given an identifier in the NOTIFYICONDATA structure pointed to by lpdata—either
        /// through its uID or guidItem member. This identifier is used in subsequent calls to Shell_NotifyIcon to perform later actions
        /// on the icon.
        /// </summary>
        NIM_ADD = 0x00000000,

        /// <summary>
        /// Modifies an icon in the status area. NOTIFYICONDATA structure pointed to by lpdata uses the ID originally assigned to the
        /// icon when it was added to the notification area (NIM_ADD) to identify the icon to be modified.
        /// </summary>
        NIM_MODIFY = 0x00000001,

        /// <summary>
        /// Deletes an icon from the status area. NOTIFYICONDATA structure pointed to by lpdata uses the ID originally assigned to the
        /// icon when it was added to the notification area (NIM_ADD) to identify the icon to be deleted.
        /// </summary>
        NIM_DELETE = 0x00000002,

        /// <summary>
        /// Shell32.dll version 5.0 and later only. Returns focus to the taskbar notification area. Notification area icons should use
        /// this message when they have completed their UI operation. For example, if the icon displays a shortcut menu, but the user
        /// presses ESC to cancel it, use NIM_SETFOCUS to return focus to the notification area.
        /// </summary>
        NIM_SETFOCUS = 0x00000003,

        /// <summary>
        /// Shell32.dll version 5.0 and later only. Instructs the notification area to behave according to the version number specified
        /// in the uVersion member of the structure pointed to by lpdata. The version number specifies which members are recognized.
        /// <para>
        /// NIM_SETVERSION must be called every time a notification area icon is added (NIM_ADD)&gt;. It does not need to be called with
        /// NIM_MOFIDY. The version setting is not persisted once a user logs off.
        /// </para>
        /// </summary>
        NIM_SETVERSION = 0x00000004,
    }
}
