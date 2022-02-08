using System;

namespace MicaForEveryone.Win32.PInvoke
{
    public static class Macros
    {
        public const uint WM_APP = 0x8000;
        public const uint WM_USER = 0x0400;

        /// <summary>
        /// Resource ID for default application icon
        /// </summary>
        public const int IDI_APPLICATION_ICON = 32512;

        public const int MAX_PATH = 260;

        public static int GET_X_LPARAM(IntPtr lp) => unchecked((short)(long)lp);

        public static int GET_Y_LPARAM(IntPtr lp) => unchecked((short)((long)lp >> 16));

        public static ushort LOWORD(uint dwValue) => (ushort)(dwValue & 0xffff);

        public static ushort LOWORD(IntPtr dwValue) => unchecked((ushort)(long)dwValue);

        public static ushort LOWORD(UIntPtr dwValue) => unchecked((ushort)(ulong)dwValue);

        public static uint MAKELONG(ushort wLow, ushort wHigh) => ((uint)wHigh << 16) | ((uint)wLow & 0xffff);

        public static IntPtr MAKELPARAM(ushort wLow, ushort wHigh) => new IntPtr(MAKELONG(wLow, wHigh));

        public static ushort MAKEWORD(byte bLow, byte bHigh) => (ushort)(bHigh << 8 | bLow & 0xff);

        public static IntPtr MAKEINTATOM(ushort i) => new IntPtr(unchecked((int)MAKELONG(i, 0)));
    }
}
