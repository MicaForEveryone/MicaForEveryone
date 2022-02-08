using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WNDCLASSEX
    {
        /// <summary>
        /// <para>Type: <c>UINT</c></para>
        /// <para>
        /// The size, in bytes, of this structure. Set this member to . Be sure to set this member before calling the GetClassInfoEx function.
        /// </para>
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// <para>Type: <c>UINT</c></para>
        /// <para>The class style(s). This member can be any combination of the Class Styles.</para>
        /// </summary>
        public WindowClassStyles style;

        /// <summary>
        /// <para>Type: <c>WNDPROC</c></para>
        /// <para>
        /// A pointer to the window procedure. You must use the CallWindowProc function to call the window procedure. For more
        /// information, see WindowProc.
        /// </para>
        /// </summary>
        public WndProc lpfnWndProc;

        /// <summary>
        /// <para>Type: <c>int</c></para>
        /// <para>The number of extra bytes to allocate following the window-class structure. The system initializes the bytes to zero.</para>
        /// </summary>
        public int cbClsExtra;

        /// <summary>
        /// <para>Type: <c>int</c></para>
        /// <para>
        /// The number of extra bytes to allocate following the window instance. The system initializes the bytes to zero. If an
        /// application uses <c>WNDCLASSEX</c> to register a dialog box created by using the <c>CLASS</c> directive in the resource file,
        /// it must set this member to <c>DLGWINDOWEXTRA</c>.
        /// </para>
        /// </summary>
        public int cbWndExtra;

        /// <summary>
        /// <para>Type: <c>HINSTANCE</c></para>
        /// <para>A handle to the instance that contains the window procedure for the class.</para>
        /// </summary>
        public IntPtr hInstance;

        /// <summary>
        /// <para>Type: <c>HICON</c></para>
        /// <para>
        /// A handle to the class icon. This member must be a handle to an icon resource. If this member is <c>NULL</c>, the system
        /// provides a default icon.
        /// </para>
        /// </summary>
        public IntPtr hIcon;

        /// <summary>
        /// <para>Type: <c>HCURSOR</c></para>
        /// <para>
        /// A handle to the class cursor. This member must be a handle to a cursor resource. If this member is <c>NULL</c>, an
        /// application must explicitly set the cursor shape whenever the mouse moves into the application's window.
        /// </para>
        /// </summary>
        public IntPtr hCursor;

        /// <summary>
        /// <para>Type: <c>HBRUSH</c></para>
        /// <para>
        /// A handle to the class background brush. This member can be a handle to the brush to be used for painting the background, or
        /// it can be a color value. A color value must be one of the following standard system colors (the value 1 must be added to the
        /// chosen color). If a color value is given, you must convert it to one of the following <c>HBRUSH</c> types:
        /// </para>
        /// <list type="bullet">
        /// <item>
        /// <term>COLOR_ACTIVEBORDER</term>
        /// </item>
        /// <item>
        /// <term>COLOR_ACTIVECAPTION</term>
        /// </item>
        /// <item>
        /// <term>COLOR_APPWORKSPACE</term>
        /// </item>
        /// <item>
        /// <term>COLOR_BACKGROUND</term>
        /// </item>
        /// <item>
        /// <term>COLOR_BTNFACE</term>
        /// </item>
        /// <item>
        /// <term>COLOR_BTNSHADOW</term>
        /// </item>
        /// <item>
        /// <term>COLOR_BTNTEXT</term>
        /// </item>
        /// <item>
        /// <term>COLOR_CAPTIONTEXT</term>
        /// </item>
        /// <item>
        /// <term>COLOR_GRAYTEXT</term>
        /// </item>
        /// <item>
        /// <term>COLOR_HIGHLIGHT</term>
        /// </item>
        /// <item>
        /// <term>COLOR_HIGHLIGHTTEXT</term>
        /// </item>
        /// <item>
        /// <term>COLOR_INACTIVEBORDER</term>
        /// </item>
        /// <item>
        /// <term>COLOR_INACTIVECAPTION</term>
        /// </item>
        /// <item>
        /// <term>COLOR_MENU</term>
        /// </item>
        /// <item>
        /// <term>COLOR_MENUTEXT</term>
        /// </item>
        /// <item>
        /// <term>COLOR_SCROLLBAR</term>
        /// </item>
        /// <item>
        /// <term>COLOR_WINDOW</term>
        /// </item>
        /// <item>
        /// <term>COLOR_WINDOWFRAME</term>
        /// </item>
        /// <item>
        /// <term>COLOR_WINDOWTEXT</term>
        /// </item>
        /// </list>
        /// <para>The system automatically deletes class background brushes when the class is unregistered by using</para>
        /// <para>UnregisterClass</para>
        /// <para>. An application should not delete these brushes.</para>
        /// <para>
        /// When this member is <c>NULL</c>, an application must paint its own background whenever it is requested to paint in its client
        /// area. To determine whether the background must be painted, an application can either process the WM_ERASEBKGND message or
        /// test the <c>fErase</c> member of the PAINTSTRUCT structure filled by the BeginPaint function.
        /// </para>
        /// </summary>
        public IntPtr hbrBackground;

        /// <summary>
        /// <para>Type: <c>LPCTSTR</c></para>
        /// <para>
        /// Pointer to a null-terminated character string that specifies the resource name of the class menu, as the name appears in the
        /// resource file. If you use an integer to identify the menu, use the MAKEINTRESOURCE macro. If this member is <c>NULL</c>,
        /// windows belonging to this class have no default menu.
        /// </para>
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszMenuName;

        /// <summary>
        /// <para>Type: <c>LPCTSTR</c></para>
        /// <para>
        /// A pointer to a null-terminated string or is an atom. If this parameter is an atom, it must be a class atom created by a
        /// previous call to the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of
        /// <c>lpszClassName</c>; the high-order word must be zero.
        /// </para>
        /// <para>
        /// If <c>lpszClassName</c> is a string, it specifies the window class name. The class name can be any name registered with
        /// RegisterClass or RegisterClassEx, or any of the predefined control-class names.
        /// </para>
        /// <para>
        /// The maximum length for <c>lpszClassName</c> is 256. If <c>lpszClassName</c> is greater than the maximum length, the
        /// RegisterClassEx function will fail.
        /// </para>
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszClassName;

        /// <summary>
        /// <para>Type: <c>HICON</c></para>
        /// <para>
        /// A handle to a small icon that is associated with the window class. If this member is <c>NULL</c>, the system searches the
        /// icon resource specified by the <c>hIcon</c> member for an icon of the appropriate size to use as the small icon.
        /// </para>
        /// </summary>
        public IntPtr hIconSm;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate IntPtr WndProc([In] IntPtr hwnd, [In] uint uMsg, [In] IntPtr wParam, [In] IntPtr lParam);
}
