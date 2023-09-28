using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.PInvoke;

public unsafe static partial class GDI
{
    public readonly struct HRGN
    {
        public readonly void* Value;

        public HRGN(void* value)
        {
            Value = value;
        }

        public static HRGN INVALID_VALUE => new HRGN((void*)(-1));

        public static HRGN NULL => new HRGN(null);
    }

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern HRGN CreateRectRgn(int x, int y, int w, int h);

    [LibraryImport("user32.dll")]
    public static partial int SetWindowRgn(HWND hWnd, HRGN hRgn, [MarshalAs(UnmanagedType.Bool)]bool bRedraw);
}
