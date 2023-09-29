using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Modules;
using static MicaForEveryone.PInvoke.Generic;

namespace MicaForEveryone.PInvoke;

public static unsafe partial class Windowing
{
    #region Type Declarations
    public readonly struct HWND : IEquatable<HWND>, IFormattable
    {
        public readonly void* Value;

        public HWND(void* value)
        {
            Value = value;
        }

        public static HWND INVALID_VALUE => new HWND((void*)(-1));

        public static HWND NULL => new HWND(null);

        public static bool operator ==(HWND left, HWND right) => left.Value == right.Value;

        public static bool operator !=(HWND left, HWND right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is HWND other) && Equals(other);

        public bool Equals(HWND other) => ((nuint)(Value)).Equals((nuint)(other.Value));

        public override int GetHashCode() => ((nuint)(Value)).GetHashCode();

        public override string ToString() => ((nuint)(Value)).ToString((sizeof(nint) == 4) ? "X8" : "X16");

        public string ToString(string? format, IFormatProvider? formatProvider) => ((nuint)(Value)).ToString(format, formatProvider);
    }

    [Flags]
    public enum WindowStylesEx : uint
    {
        WS_EX_NOACTIVATE = 0x08000000,

        /// <summary>
        /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is
        /// deactivated. To add or remove this style, use the SetWindowPos function.
        /// </summary>
        WS_EX_TOPMOST = 0x00000008,
    }

    [Flags]
    public enum WindowStyles : uint
    {
        /// <summary>The window has a thin-line border.</summary>
        WS_BORDER = 0x800000,

        /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
        WS_CAPTION = 0xc00000,

        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.
        /// </summary>
        WS_CHILD = 0x40000000,

        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating
        /// the parent window.
        /// </summary>
        WS_CLIPCHILDREN = 0x2000000,

        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the
        /// WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If
        /// WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child
        /// window, to draw within the client area of a neighboring child window.
        /// </summary>
        WS_CLIPSIBLINGS = 0x4000000,

        /// <summary>
        /// The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has
        /// been created, use the EnableWindow function.
        /// </summary>
        WS_DISABLED = 0x8000000,

        /// <summary>
        /// The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
        /// </summary>
        WS_DLGFRAME = 0x400000,

        /// <summary>
        /// The window is the first control of a group of controls. The group consists of this first control and all controls defined
        /// after it, up to the next control with the WS_GROUP style. The first control in each group usually has the WS_TABSTOP style so
        /// that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group
        /// to the next control in the group by using the direction keys. You can turn this style on and off to change dialog box
        /// navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// </summary>
        WS_GROUP = 0x20000,

        /// <summary>The window has a horizontal scroll bar.</summary>
        WS_HSCROLL = 0x100000,

        /// <summary>The window is initially maximized.</summary>
        WS_MAXIMIZE = 0x1000000,

        /// <summary>
        /// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
        /// </summary>
        WS_MAXIMIZEBOX = 0x10000,

        /// <summary>The window is initially minimized.</summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>
        /// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
        /// </summary>
        WS_MINIMIZEBOX = 0x20000,

        /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
        WS_OVERLAPPED = 0x0,

        /// <summary>The window is an overlapped window.</summary>
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
        WS_POPUP = 0x80000000u,

        /// <summary>
        /// The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.
        /// </summary>
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        /// <summary>The window has a sizing border.</summary>
        WS_THICKFRAME = 0x40000,

        /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
        WS_SYSMENU = 0x80000,

        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes
        /// the keyboard focus to the next control with the WS_TABSTOP style. You can turn this style on and off to change dialog box
        /// navigation. To change this style after a window has been created, use the SetWindowLong function. For user-created windows
        /// and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
        /// </summary>
        WS_TABSTOP = 0x10000,

        /// <summary>
        /// The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.
        /// </summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>The window has a vertical scroll bar.</summary>
        WS_VSCROLL = 0x200000,

        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style.
        /// </summary>
        WS_TILED = WS_OVERLAPPED,

        /// <summary>The window is initially minimized. Same as the WS_MINIMIZE style.</summary>
        WS_ICONIC = WS_MINIMIZE,

        /// <summary>The window has a sizing border. Same as the WS_THICKFRAME style.</summary>
        WS_SIZEBOX = WS_THICKFRAME,

        /// <summary>The window is an overlapped window. Same as the WS_OVERLAPPEDWINDOW style.</summary>
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

        /// <summary>Same as the WS_CHILD style.</summary>
        WS_CHILDWINDOW = WS_CHILD,
    }

    [Flags]
    public enum WindowClassStyles : uint
    {
        /// <summary>
        /// Aligns the window's client area on a byte boundary (in the x direction). This style affects the width of the window and its
        /// horizontal placement on the display.
        /// </summary>
        CS_BYTEALIGNCLIENT = 0x1000,

        /// <summary>
        /// Aligns the window on a byte boundary (in the x direction). This style affects the width of the window and its horizontal
        /// placement on the display.
        /// </summary>
        CS_BYTEALIGNWINDOW = 0x2000,

        /// <summary>
        /// Allocates one device context to be shared by all windows in the class. Because window classes are process specific, it is
        /// possible for multiple threads of an application to create a window of the same class. It is also possible for the threads to
        /// attempt to use the device context simultaneously. When this happens, the system allows only one thread to successfully finish
        /// its drawing operation.
        /// </summary>
        CS_CLASSDC = 0x0040,

        /// <summary>
        /// Sends a double-click message to the window procedure when the user double-clicks the mouse while the cursor is within a
        /// window belonging to the class.
        /// </summary>
        CS_DBLCLKS = 0x0008,

        /// <summary>
        /// Enables the drop shadow effect on a window. The effect is turned on and off through SPI_SETDROPSHADOW. Typically, this is
        /// enabled for small, short-lived windows such as menus to emphasize their Z-order relationship to other windows. Windows
        /// created from a class with this style must be top-level windows; they may not be child windows.
        /// </summary>
        CS_DROPSHADOW = 0x00020000,

        /// <summary>
        /// Indicates that the window class is an application global class. For more information, see the "Application Global Classes"
        /// section of About Window Classes.
        /// </summary>
        CS_GLOBALCLASS = 0x4000,

        /// <summary>Redraws the entire window if a movement or size adjustment changes the width of the client area.</summary>
        CS_HREDRAW = 0x0002,

        /// <summary>Disables Close on the window menu.</summary>
        CS_NOCLOSE = 0x0200,

        /// <summary>Allocates a unique device context for each window in the class.</summary>
        CS_OWNDC = 0x0020,

        /// <summary>
        /// Sets the clipping rectangle of the child window to that of the parent window so that the child can draw on the parent. A
        /// window with the CS_PARENTDC style bit receives a regular device context from the system's cache of device contexts. It does
        /// not give the child the parent's device context or device context settings. Specifying CS_PARENTDC enhances an application's performance.
        /// </summary>
        CS_PARENTDC = 0x0080,

        /// <summary>
        /// Saves, as a bitmap, the portion of the screen image obscured by a window of this class. When the window is removed, the
        /// system uses the saved bitmap to restore the screen image, including other windows that were obscured. Therefore, the system
        /// does not send WM_PAINT messages to windows that were obscured if the memory used by the bitmap has not been discarded and if
        /// other screen actions have not invalidated the stored image.
        /// <para>
        /// This style is useful for small windows (for example, menus or dialog boxes) that are displayed briefly and then removed
        /// before other screen activity takes place. This style increases the time required to display the window, because the system
        /// must first allocate memory to store the bitmap.
        /// </para>
        /// </summary>
        CS_SAVEBITS = 0x0800,

        /// <summary>Redraws the entire window if a movement or size adjustment changes the height of the client area.</summary>
        CS_VREDRAW = 0x0001,

        /// <summary>Undocumented.</summary>
        CS_IME = 0x00010000,
    }

    public enum WindowLongIndex : int
    {
        GWL_WNDPROC = -4,
        GWL_USERDATA = -21
    }

    public readonly unsafe partial struct HICON
    {
        public readonly void* Value;

        public HICON(void* value)
        {
            Value = value;
        }

        public static HICON INVALID_VALUE => new HICON((void*)(-1));

        public static HICON NULL => new HICON(null);
    }

    public readonly unsafe partial struct HMENU
    {
        public readonly void* Value;

        public HMENU(void* value)
        {
            Value = value;
        }

        public static HMENU INVALID_VALUE => new HMENU((void*)(-1));

        public static HMENU NULL => new HMENU(null);
    }

    public readonly unsafe partial struct HCURSOR
    {
        public readonly void* Value;

        public HCURSOR(void* value)
        {
            Value = value;
        }

        public static HCURSOR INVALID_VALUE => new HCURSOR((void*)(-1));

        public static HCURSOR NULL => new HCURSOR(null);
    }

    public readonly unsafe partial struct HBRUSH
    {
        public readonly void* Value;

        public HBRUSH(void* value)
        {
            Value = value;
        }

        public static HBRUSH INVALID_VALUE => new HBRUSH((void*)(-1));

        public static HBRUSH NULL => new HBRUSH(null);
    }

    public readonly struct WPARAM
    {
        public readonly nuint Value;

        public WPARAM(nuint value)
        {
            Value = value;
        }

        public static implicit operator nuint(WPARAM value) => value.Value;
    }

    public readonly struct LPARAM
    {
        public readonly nint Value;

        public LPARAM(nint value)
        {
            Value = value;
        }

        public static implicit operator nint(LPARAM value) => value.Value;
    }

    public readonly partial struct LRESULT : IComparable, IComparable<LRESULT>, IEquatable<LRESULT>, IFormattable
    {
        public readonly nint Value;

        public LRESULT(nint value)
        {
            Value = value;
        }

        public static bool operator ==(LRESULT left, LRESULT right) => left.Value == right.Value;

        public static bool operator !=(LRESULT left, LRESULT right) => left.Value != right.Value;

        public static bool operator <(LRESULT left, LRESULT right) => left.Value < right.Value;

        public static bool operator <=(LRESULT left, LRESULT right) => left.Value <= right.Value;

        public static bool operator >(LRESULT left, LRESULT right) => left.Value > right.Value;

        public static bool operator >=(LRESULT left, LRESULT right) => left.Value >= right.Value;

        public static implicit operator LRESULT(byte value) => new LRESULT(value);

        public static explicit operator byte(LRESULT value) => (byte)(value.Value);

        public static implicit operator LRESULT(short value) => new LRESULT(value);

        public static explicit operator short(LRESULT value) => (short)(value.Value);

        public static implicit operator LRESULT(int value) => new LRESULT(value);

        public static explicit operator int(LRESULT value) => (int)(value.Value);

        public static explicit operator LRESULT(long value) => new LRESULT(unchecked((nint)(value)));

        public static implicit operator long(LRESULT value) => value.Value;

        public static implicit operator LRESULT(nint value) => new LRESULT(value);

        public static implicit operator nint(LRESULT value) => value.Value;

        public static implicit operator LRESULT(sbyte value) => new LRESULT(value);

        public static explicit operator sbyte(LRESULT value) => (sbyte)(value.Value);

        public static implicit operator LRESULT(ushort value) => new LRESULT(value);

        public static explicit operator ushort(LRESULT value) => (ushort)(value.Value);

        public static explicit operator LRESULT(uint value) => new LRESULT(unchecked((nint)(value)));

        public static explicit operator uint(LRESULT value) => (uint)(value.Value);

        public static explicit operator LRESULT(ulong value) => new LRESULT(unchecked((nint)(value)));

        public static explicit operator ulong(LRESULT value) => (ulong)(value.Value);

        public static explicit operator LRESULT(nuint value) => new LRESULT(unchecked((nint)(value)));

        public static explicit operator nuint(LRESULT value) => (nuint)(value.Value);

        public int CompareTo(object? obj)
        {
            if (obj is LRESULT other)
            {
                return CompareTo(other);
            }

            return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of LRESULT.");
        }

        public int CompareTo(LRESULT other) => Value.CompareTo(other.Value);

        public override bool Equals(object? obj) => (obj is LRESULT other) && Equals(other);

        public bool Equals(LRESULT other) => Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();

        public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
    }

    public unsafe partial struct WNDCLASSEXW
    {
        public uint cbSize;

        public WindowClassStyles style;

        public delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> lpfnWndProc;

        public int cbClsExtra;

        public int cbWndExtra;

        public HINSTANCE hInstance;

        public HICON hIcon;

        public HCURSOR hCursor;

        public HBRUSH hbrBackground;

        public ushort* lpszMenuName;

        public ushort* lpszClassName;

        public HICON hIconSm;
    }

    public unsafe partial struct CREATESTRUCTW
    {
        public void* lpCreateParams;

        public HINSTANCE hInstance;

        public HMENU hMenu;

        public HWND hwndParent;

        public int cy;

        public int cx;

        public int y;

        public int x;

        public int style;

        public ushort* lpszName;

        public ushort* lpszClass;

        public uint dwExStyle;
    }
    #endregion

    #region Methods

    [LibraryImport("user32.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static unsafe partial HWND CreateWindowExW(WindowStylesEx dwExStyle, string lpClassName, string? lpWindowName, WindowStyles dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, void* hMenu, HINSTANCE hInstance, void* lpParam);

    [DllImport("user32.dll", EntryPoint = "RegisterClassExW", ExactSpelling = true)]
    public static extern ushort RegisterClassExW(WNDCLASSEXW* param0);

    [DllImport("user32.dll", EntryPoint = "DefWindowProcW", ExactSpelling = true)]
    public static extern LRESULT DefWindowProcW(HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam);

    [DllImport("user32.dll", EntryPoint = "DestroyWindow", ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DestroyWindow(HWND hWnd);

    [DllImport("user32,dll", ExactSpelling = true)]
    public static extern int GetWindowLongW(HWND hWnd, WindowLongIndex nIndex);

    public static nint GetWindowLongPtrW(HWND hWnd, WindowLongIndex nIndex)
    {
        if (sizeof(nint) == 4)
        {
            return GetWindowLongW(hWnd, nIndex);
        }
        else
        {
            [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", ExactSpelling = true)]
            static extern nint _GetWindowLongPtrW(HWND hWnd, WindowLongIndex nIndex);

            return _GetWindowLongPtrW(hWnd, nIndex);
        }
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumWindows(delegate* unmanaged<HWND, LPARAM, BOOL> lpEnumFunc, LPARAM lParam);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern LRESULT CallWindowProcW(delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT> lpPrevWndFunc, HWND hWnd, uint Msg, WPARAM wParam, LPARAM lParam);


    [DllImport("user32.dll", EntryPoint = "SetWindowLong", ExactSpelling = true)]
    public static extern int SetWindowLongW(HWND hWnd, WindowLongIndex nIndex, int dwNewLong);

    public static nint SetWindowLongPtrW(HWND hWnd, WindowLongIndex nIndex, nint dwNewLong)
    {
        if (sizeof(nint) == 4)
        {
            return SetWindowLongW(hWnd, nIndex, (int)dwNewLong);
        }
        else
        {
            [DllImport("user32", EntryPoint = "SetWindowLongPtrW", ExactSpelling = true)]
            static extern nint _SetWindowLongPtrW(HWND hWnd, WindowLongIndex nIndex, nint dwNewLong);

            return _SetWindowLongPtrW(hWnd, nIndex, dwNewLong);
        }
    }

    [DllImport("comctl32.dll", ExactSpelling = true)]
    public static extern unsafe HRESULT LoadIconMetric(HINSTANCE hInstance, ushort* pszName, int lims, HICON* phico);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(HWND hWnd, int nCmdShow);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(HWND hWnd, HWND hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsIconic(HWND hWnd);
    #endregion

    #region Constants
    public const int WM_APP = 0x8000;
    public const int WM_USER = 0x0400;
    public const int WM_LBUTTONUP = 0x202;
    public const int WM_DESTROY = 0x0002;
    public const int WM_QUIT = 0x0012;

    public static ushort* IDI_APPLICATION => ((ushort*)((nuint)((ushort)(32512))));

    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_NOACTIVATE = 0x0010;

    public const int SW_RESTORE = 9;

    public const int LIM_SMALL = 0;
    public const int LIM_LARGE = 1;
    #endregion
}
