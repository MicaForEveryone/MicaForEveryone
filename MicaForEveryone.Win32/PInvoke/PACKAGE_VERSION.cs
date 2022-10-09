using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PACKAGE_VERSION
    {
        /// <summary>
        /// <para>Type: <c>UINT64</c></para>
        /// <para>The full version number of the package represented as a single integral value.</para>
        /// </summary>
        [FieldOffset(0)]
        public ulong Version;

        /// <summary>Parts of the Version.</summary>
        [FieldOffset(0)]
        public DUMMYSTRUCTNAME Parts;

        /// <summary>Parts of the Version.</summary>
        public struct DUMMYSTRUCTNAME
        {
            /// <summary>
            /// <para>Type: <c>USHORT</c></para>
            /// <para>The revision version number of the package.</para>
            /// </summary>
            public ushort Revision;

            /// <summary>
            /// <para>Type: <c>USHORT</c></para>
            /// <para>The build version number of the package.</para>
            /// </summary>
            public ushort Build;

            /// <summary>
            /// <para>Type: <c>USHORT</c></para>
            /// <para>The minor version number of the package.</para>
            /// </summary>
            public ushort Minor;

            /// <summary>
            /// <para>Type: <c>USHORT</c></para>
            /// <para>The major version number of the package.</para>
            /// </summary>
            public ushort Major;
        }
    }
}