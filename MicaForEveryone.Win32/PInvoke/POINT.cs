using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Win32.PInvoke
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
