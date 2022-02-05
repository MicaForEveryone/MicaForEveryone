using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYICONDATA
    {
        /// <summary>Size of this structure, in bytes.</summary>
        public uint cbSize;

        /// <summary>
        /// Handle to the window that receives notification messages associated with an icon in the taskbar status area. The Shell uses
        /// hWnd and uID to identify which icon to operate on when Shell_NotifyIcon is invoked.
        /// </summary>
        public IntPtr hwnd;

        /// <summary>
        /// Application-defined identifier of the taskbar icon. The Shell uses hWnd and uID to identify which icon to operate on when
        /// Shell_NotifyIcon is invoked. You can have multiple icons associated with a single hWnd by assigning each a different uID.
        /// </summary>
        public uint uID;

        /// <summary>
        /// Flags that indicate which of the other members contain valid data. This member can be a combination of the NIF_XXX constants.
        /// </summary>
        public NIF uFlags;

        /// <summary>
        /// Application-defined message identifier. The system uses this identifier to send notifications to the window identified in hWnd.
        /// </summary>
        public uint uCallbackMessage;

        /// <summary>Handle to the icon to be added, modified, or deleted.</summary>
        public IntPtr hIcon;

        /// <summary>
        /// String with the text for a standard ToolTip. It can have a maximum of 64 characters including the terminating NULL. For
        /// Version 5.0 and later, szTip can have a maximum of 128 characters, including the terminating NULL.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;

        /// <summary>State of the icon.</summary>
        public NIS dwState;

        /// <summary>
        /// A value that specifies which bits of the state member are retrieved or modified. For example, setting this member to
        /// NIS_HIDDEN causes only the item's hidden state to be retrieved.
        /// </summary>
        public NIS dwStateMask;

        /// <summary>
        /// String with the text for a balloon ToolTip. It can have a maximum of 255 characters. To remove the ToolTip, set the NIF_INFO
        /// flag in uFlags and set szInfo to an empty string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;

        /// <summary>
        /// NOTE: This field is also used for the Timeout value. Specifies whether the Shell notify icon interface should use Windows 95
        /// or Windows 2000 behavior. For more information on the differences in these two behaviors, see Shell_NotifyIcon. This member
        /// is only employed when using Shell_NotifyIcon to send an NIM_VERSION message.
        /// </summary>
        public int uTimeoutOrVersion;

        /// <summary>
        /// String containing a title for a balloon ToolTip. This title appears in boldface above the text. It can have a maximum of 63 characters.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;

        /// <summary>
        /// Adds an icon to a balloon ToolTip. It is placed to the left of the title. If the szTitleInfo member is zero-length, the icon
        /// is not shown.
        /// </summary>
        public NIIF dwInfoFlags;

        /// <summary>
        /// <para>
        /// Windows 7 and later: A registered GUID that identifies the icon.This value overrides uID and is the recommended method of
        /// identifying the icon.The NIF_GUID flag must be set in the uFlags member.
        /// </para>
        /// <para>Windows XP and Windows Vista: Reserved; must be set to 0.</para>
        /// <para>
        /// If your application is intended to run on both Windows Vista and Windows 7, it is imperative that you check the version of
        /// Windows and only specify a nonzero guidItem if on Windows 7 or later.
        /// </para>
        /// <para>
        /// If you identify the notification icon with a GUID in one call to Shell_NotifyIcon, you must use that same GUID to identify
        /// the icon in any subsequent Shell_NotifyIcon calls that deal with that same icon.
        /// </para>
        /// <para>To generate a GUID for use in this member, use a GUID-generating tool such as Guidgen.exe.</para>
        /// </summary>
        public Guid guidItem;

        /// <summary>
        /// Windows Vista and later. The handle of a customized notification icon provided by the application that should be used
        /// independently of the notification area icon. If this member is non-NULL and the NIIF_USER flag is set in the dwInfoFlags
        /// member, this icon is used as the notification icon. If this member is NULL, the legacy behavior is carried out.
        /// </summary>
        public IntPtr hBalloonIcon;
    }

    [Flags]
    public enum NIS
    {
        /// <summary>The icon is hidden.</summary>
        NIS_HIDDEN = 0x00000001,

        /// <summary>The icon resource is shared between multiple icons.</summary>
        NIS_SHAREDICON = 0x00000002
    }

    [Flags]
    public enum NIIF
    {
        /// <summary>No icon.</summary>
        NIIF_NONE = 0x00000000,

        /// <summary>An information icon.</summary>
        NIIF_INFO = 0x00000001,

        /// <summary>A warning icon.</summary>
        NIIF_WARNING = 0x00000002,

        /// <summary>An error icon.</summary>
        NIIF_ERROR = 0x00000003,

        /// <summary>
        /// Windows XP SP2 and later.
        /// <list type="bullet">
        /// <item>
        /// <description>Windows XP: Use the icon identified in hIcon as the notification balloon's title icon.</description>
        /// </item>
        /// <item>
        /// <description>
        /// Windows Vista and later: Use the icon identified in hBalloonIcon as the notification balloon's title icon.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        NIIF_USER = 0x00000004,

        /// <summary>Windows XP and later. Reserved.</summary>
        NIIF_ICON_MASK = 0x0000000F,

        /// <summary>Windows XP and later. Do not play the associated sound. Applies only to notifications.</summary>
        NIIF_NOSOUND = 0x00000010,

        /// <summary>
        /// Windows Vista and later. The large version of the icon should be used as the notification icon. This corresponds to the icon
        /// with dimensions SM_CXICON x SM_CYICON. If this flag is not set, the icon with dimensions XM_CXSMICON x SM_CYSMICON is used.
        /// <list type="bullet">
        /// <item>
        /// <description>This flag can be used with all stock icons.</description>
        /// </item>
        /// <item>
        /// <description>
        /// Applications that use older customized icons (NIIF_USER with hIcon) must provide a new SM_CXICON x SM_CYICON version in the
        /// tray icon(hIcon). These icons are scaled down when they are displayed in the System Tray or System Control Area(SCA).
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// New customized icons(NIIF_USER with hBalloonIcon) must supply an SM_CXICON x SM_CYICON version in the supplied icon(hBalloonIcon).
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        NIIF_LARGE_ICON = 0x00000020,

        /// <summary>
        /// Windows 7 and later. Do not display the balloon notification if the current user is in "quiet time", which is the first hour
        /// after a new user logs into his or her account for the first time. During this time, most notifications should not be sent or
        /// shown. This lets a user become accustomed to a new computer system without those distractions. Quiet time also occurs for
        /// each user after an operating system upgrade or clean installation. A notification sent with this flag during quiet time is
        /// not queued; it is simply dismissed unshown. The application can resend the notification later if it is still valid at that time.
        /// <para>
        /// Because an application cannot predict when it might encounter quiet time, we recommended that this flag always be set on all
        /// appropriate notifications by any application that means to honor quiet time.&gt;
        /// </para>
        /// <para>
        /// During quiet time, certain notifications should still be sent because they are expected by the user as feedback in response
        /// to a user action, for instance when he or she plugs in a USB device or prints a document.
        /// </para>
        /// <para>If the current user is not in quiet time, this flag has no effect.</para>
        /// </summary>
        NIIF_RESPECT_QUIET_TIME = 0x00000080
    }
}
