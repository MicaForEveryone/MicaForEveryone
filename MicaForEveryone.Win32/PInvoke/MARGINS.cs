using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        /// <summary>Width of the left border that retains its size.</summary>
        public int cxLeftWidth;

        /// <summary>Width of the right border that retains its size.</summary>
        public int cxRightWidth;

        /// <summary>Height of the top border that retains its size.</summary>
        public int cyTopHeight;

        /// <summary>Height of the bottom border that retains its size.</summary>
        public int cyBottomHeight;

        public MARGINS(int allMargins) => cxLeftWidth = cxRightWidth = cyTopHeight = cyBottomHeight = allMargins;
    }
}
