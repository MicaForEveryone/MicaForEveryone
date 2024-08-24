using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.PInvoke;

public static partial class NotifyIcon
{
#if IA32
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe partial struct NOTIFYICONDATAW
    {
        public uint cbSize;

        public HWND hWnd;

        public uint uID;

        public uint uFlags;

        public uint uCallbackMessage;

        public HICON hIcon;

        public fixed ushort szTip[128];

        public uint dwState;

        public uint dwStateMask;

        public fixed ushort szInfo[256];

        public _Anonymous_e__Union Anonymous;

        public fixed ushort szInfoTitle[64];

        public uint dwInfoFlags;

        public Guid guidItem;

        public HICON hBalloonIcon;

        [UnscopedRef]
        public ref uint uTimeout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.uTimeout;
            }
        }

        [UnscopedRef]
        public ref uint uVersion
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.uVersion;
            }
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public uint uTimeout;

            [FieldOffset(0)]
            public uint uVersion;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public partial struct NOTIFYICONIDENTIFIER
    {
        public uint cbSize;

        public HWND hWnd;

        public uint uID;

        public Guid guidItem;
    }
#else
    public unsafe partial struct NOTIFYICONDATAW
    {
        public uint cbSize;

        public HWND hWnd;

        public uint uID;

        public uint uFlags;

        public uint uCallbackMessage;

        public HICON hIcon;

        public fixed ushort szTip[128];

        public uint dwState;

        public uint dwStateMask;

        public fixed ushort szInfo[256];

        public _Anonymous_e__Union Anonymous;

        public fixed ushort szInfoTitle[64];

        public uint dwInfoFlags;

        public Guid guidItem;

        public HICON hBalloonIcon;

        [UnscopedRef]
        public ref uint uTimeout
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.uTimeout;
            }
        }

        [UnscopedRef]
        public ref uint uVersion
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref Anonymous.uVersion;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            public uint uTimeout;

            [FieldOffset(0)]
            public uint uVersion;
        }
    }

    public partial struct NOTIFYICONIDENTIFIER
    {
        public uint cbSize;

        public HWND hWnd;

        public uint uID;

        public Guid guidItem;
    }
#endif

    public const int NIF_MESSAGE = 0x00000001;
    public const int NIF_ICON = 0x00000002;
    public const int NIF_TIP = 0x00000004;
    public const int NIF_SHOWTIP = 0x00000080;

    public const int NIM_ADD = 0x00000000;
    public const int NIM_DELETE = 0x00000002;
    public const int NIM_SETVERSION = 0x00000004;

    public const int NIN_SELECT = WM_USER;
    public const int NINF_KEY = 0x1;
    public const int NIN_KEYSELECT = NIN_SELECT | NINF_KEY;

    public const int NIN_POPUPOPEN = 0x406;
    public const int NIN_POPUPCLOSE = 0x407;

    public const int WM_CONTEXTMENU = 0x7B;

    [LibraryImport("shell32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static unsafe partial bool Shell_NotifyIconW(uint dwMessage, NOTIFYICONDATAW* lpData);

    [DllImport("shell32.dll", ExactSpelling = true)]
    public static extern unsafe HRESULT Shell_NotifyIconGetRect(NOTIFYICONIDENTIFIER* identifier, RECT* iconLocation);
}
