using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Generic;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.PInvoke;

public unsafe static partial class Messaging
{
    public partial struct MSG
    {
        public HWND hwnd;

        public uint message;

        public WPARAM wParam;

        public LPARAM lParam;

        public uint time;

        public POINT pt;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PeekMessageW(MSG* lpMsg, HWND hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern BOOL TranslateMessage(MSG* lpMsg);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern LRESULT DispatchMessageW(MSG* lpMsg);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern void PostQuitMessage(int nExitCode);
}
