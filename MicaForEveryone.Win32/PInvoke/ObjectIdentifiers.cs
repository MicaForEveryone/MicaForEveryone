namespace MicaForEveryone.Win32.PInvoke
{
    public static class ObjectIdentifiers
    {
        /// <summary>The window itself rather than a child object.</summary>
        public const int OBJID_WINDOW = unchecked((int)0x00000000);

        /// <summary>The window's system menu.</summary>
        public const int OBJID_SYSMENU = unchecked((int)0xFFFFFFFF);

        /// <summary>The window's title bar.</summary>
        public const int OBJID_TITLEBAR = unchecked((int)0xFFFFFFFE);

        /// <summary>The window's menu bar.</summary>
        public const int OBJID_MENU = unchecked((int)0xFFFFFFFD);

        /// <summary>
        /// The window's client area. In most cases, the operating system controls the frame elements and the client object contains all
        /// elements that are controlled by the application. Servers only process the WM_GETOBJECT messages in which the lParam is
        /// OBJID_CLIENT, OBJID_WINDOW, or a custom object identifier.
        /// </summary>
        public const int OBJID_CLIENT = unchecked((int)0xFFFFFFFC);

        /// <summary>The window's vertical scroll bar.</summary>
        public const int OBJID_VSCROLL = unchecked((int)0xFFFFFFFB);

        /// <summary>The window's horizontal scroll bar.</summary>
        public const int OBJID_HSCROLL = unchecked((int)0xFFFFFFFA);

        /// <summary>The window's size grip: an optional frame component located at the lower-right corner of the window frame.</summary>
        public const int OBJID_SIZEGRIP = unchecked((int)0xFFFFFFF9);

        /// <summary>The text insertion bar (caret) in the window.</summary>
        public const int OBJID_CARET = unchecked((int)0xFFFFFFF8);

        /// <summary>The mouse pointer. There is only one mouse pointer in the system, and it is not a child of any window.</summary>
        public const int OBJID_CURSOR = unchecked((int)0xFFFFFFF7);

        /// <summary>
        /// An alert that is associated with a window or an application. System provided message boxes are the only UI elements that
        /// send events with this object identifier. Server applications cannot use the AccessibleObjectFromX functions with this object
        /// identifier. This is a known issue with Microsoft Active Accessibility.
        /// </summary>
        public const int OBJID_ALERT = unchecked((int)0xFFFFFFF6);

        /// <summary>
        /// A sound object. Sound objects do not have screen locations or children, but they do have name and state attributes. They are
        /// children of the application that is playing the sound.
        /// </summary>
        public const int OBJID_SOUND = unchecked((int)0xFFFFFFF5);

        /// <summary>
        /// An object identifier that Oleacc.dll uses internally. For more information, see Appendix F: Object Identifier Values for OBJID_QUERYCLASSNAMEIDX.
        /// </summary>
        public const int OBJID_QUERYCLASSNAMEIDX = unchecked((int)0xFFFFFFF4);

        /// <summary>
        /// In response to this object identifier, third-party applications can expose their own object model. Third-party applications
        /// can return any COM interface in response to this object identifier.
        /// </summary>
        public const int OBJID_NATIVEOM = unchecked((int)0xFFFFFFF0);
    }
}
