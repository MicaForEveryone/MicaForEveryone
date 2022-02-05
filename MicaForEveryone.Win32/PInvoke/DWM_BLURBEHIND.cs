using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [Flags]
    public enum DWM_BLURBEHIND_Mask
    {
        /// <summary>A value for the fEnable member has been specified.</summary>
        DWM_BB_ENABLE = 0X00000001,

        /// <summary>A value for the hRgnBlur member has been specified.</summary>
        DWM_BB_BLURREGION = 0X00000002,

        /// <summary>A value for the fTransitionOnMaximized member has been specified.</summary>
        DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_BLURBEHIND
    {
        /// <summary>
        /// A bitwise combination of DWM Blur Behind constant values that indicates which of the members of this structure have been set.
        /// </summary>
        public DWM_BLURBEHIND_Mask dwFlags;

        /// <summary>TRUE to register the window handle to DWM blur behind; FALSE to unregister the window handle from DWM blur behind.</summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool fEnable;

        /// <summary>
        /// The region within the client area where the blur behind will be applied. A NULL value will apply the blur behind the entire
        /// client area.
        /// </summary>
        public IntPtr hRgnBlur;

        /// <summary>TRUE if the window's colorization should transition to match the maximized windows; otherwise, FALSE.</summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool fTransitionOnMaximized;

        /// <summary>Initializes a new instance of the <see cref="DWM_BLURBEHIND"/> struct.</summary>
        /// <param name="enabled">if set to <c>true</c> enabled.</param>
        public DWM_BLURBEHIND(bool enabled)
        {
            fEnable = enabled;
            hRgnBlur = IntPtr.Zero;
            fTransitionOnMaximized = false;
            dwFlags = DWM_BLURBEHIND_Mask.DWM_BB_ENABLE;
        }

        /// <summary>Gets or sets a value indicating whether the window's colorization should transition to match the maximized windows.</summary>
        /// <value><c>true</c> if the window's colorization should transition to match the maximized windows; otherwise, <c>false</c>.</value>
        public bool TransitionOnMaximized
        {
            get => fTransitionOnMaximized;
            set
            {
                fTransitionOnMaximized = value;
                dwFlags |= DWM_BLURBEHIND_Mask.DWM_BB_TRANSITIONONMAXIMIZED;
            }
        }
    }
}
