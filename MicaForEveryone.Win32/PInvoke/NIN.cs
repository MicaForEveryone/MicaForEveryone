using static MicaForEveryone.Win32.PInvoke.Macros;
using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32.PInvoke
{
    public enum NIN : uint
    {
        /// <summary>Sent when a user selects a notify icon with the mouse and activates it with the ENTER key</summary>
        NIN_SELECT = WM_USER + 0,

        /// <summary>
        /// Sent when a user selects a notify icon with the keyboard and activates it with the SPACEBAR or ENTER key, the version 5.0
        /// Shell sends the associated application an NIN_KEYSELECT notification. Earlier versions send WM_RBUTTONDOWN and WM_RBUTTONUP messages.
        /// </summary>
        NIN_KEYSELECT = NIN_SELECT | NINF_KEY,

        /// <summary>Sent when the balloon is shown (balloons are queued).</summary>
        NIN_BALLOONSHOW = WM_USER + 2,

        /// <summary>
        /// Sent when the balloon disappears. For example, when the icon is deleted. This message is not sent if the balloon is
        /// dismissed because of a timeout or if the user clicks the mouse.
        /// <para>
        /// As of Windows 7, NIN_BALLOONHIDE is also sent when a notification with the NIIF_RESPECT_QUIET_TIME flag set attempts to
        /// display during quiet time (a user's first hour on a new computer). In that case, the balloon is never displayed at all.
        /// </para>
        /// </summary>
        NIN_BALLOONHIDE = WM_USER + 3,

        /// <summary>Sent when the balloon is dismissed because of a timeout.</summary>
        NIN_BALLOONTIMEOUT = WM_USER + 4,

        /// <summary>Sent when the balloon is dismissed because the user clicked the mouse.</summary>
        NIN_BALLOONUSERCLICK = WM_USER + 5,

        /// <summary>
        /// Sent when the user hovers the cursor over an icon to indicate that the richer pop-up UI should be used in place of a
        /// standard textual tooltip.
        /// </summary>
        NIN_POPUPOPEN = WM_USER + 6,

        /// <summary>Sent when a cursor no longer hovers over an icon to indicate that the rich pop-up UI should be closed.</summary>
        NIN_POPUPCLOSE = WM_USER + 7,
    }
}
