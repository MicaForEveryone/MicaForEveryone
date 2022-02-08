using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WTA_OPTIONS
    {
        /// <summary>
        /// <para>Type: <c><c>DWORD</c></c></para>
        /// <para>A combination of flags that modify window visual style attributes. Can be a combination of the <c>WTNCA</c> constants.</para>
        /// </summary>
        public WTNCA Flags;

        /// <summary>
        /// <para>Type: <c><c>DWORD</c></c></para>
        /// <para>
        /// A bitmask that describes how the values specified in <c>dwFlags</c> should be applied. If the bit corresponding to a value in
        /// <c>dwFlags</c> is 0, that flag will be removed. If the bit is 1, the flag will be added.
        /// </para>
        /// </summary>
        public uint Mask;
    }
}
