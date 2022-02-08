using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NOTIFYICONIDENTIFIER
    {
        /// <summary>
        /// <para>Type: <c>DWORD</c></para>
        /// <para>The size of this structure, in bytes.</para>
        /// </summary>
        public uint cbSize;

        /// <summary>
        /// <para>Type: <c>HWND</c></para>
        /// <para>
        /// A handle to the parent window used by the notification's callback function. For more information, see the <see
        /// cref="NOTIFYICONDATA.hwnd"/> member of the NOTIFYICONDATA structure.
        /// </para>
        /// </summary>
        public IntPtr hWnd;

        /// <summary>
        /// <para>Type: <c>UINT</c></para>
        /// <para>
        /// The application-defined identifier of the notification icon. Multiple icons can be associated with a single <c>hWnd</c>,
        /// each with their own <c>uID</c>.
        /// </para>
        /// </summary>
        public uint uID;

        /// <summary>
        /// <para>Type: <c>GUID</c></para>
        /// <para>A registered GUID that identifies the icon. Use <c>GUID_NULL</c> if the icon is not identified by a GUID.</para>
        /// </summary>
        public Guid guidItem;

        /// <summary>Initializes a new instance of the <see cref="NOTIFYICONIDENTIFIER"/> struct.</summary>
        /// <param name="hWnd">A handle to the parent window used by the notification's callback function.</param>
        /// <param name="uID">The application-defined identifier of the notification icon.</param>
        public NOTIFYICONIDENTIFIER(IntPtr hWnd, uint uID)
        {
            cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONIDENTIFIER));
            this.hWnd = hWnd;
            this.uID = uID;
            guidItem = default;
        }

        /// <summary>Initializes a new instance of the <see cref="NOTIFYICONIDENTIFIER"/> struct.</summary>
        /// <param name="guidItem">A registered GUID that identifies the icon.</param>
        public NOTIFYICONIDENTIFIER(Guid guidItem)
        {
            cbSize = (uint)Marshal.SizeOf(typeof(NOTIFYICONIDENTIFIER));
            this.hWnd = default;
            this.uID = default;
            this.guidItem = guidItem;
        }
    }
}
