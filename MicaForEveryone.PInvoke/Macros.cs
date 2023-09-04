using System.Runtime.CompilerServices;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.PInvoke;

public class Macros
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort LOWORD(nuint l) => unchecked((ushort)(((nuint)(l)) & 0xffff));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort LOWORD(nint l) => unchecked(LOWORD((nuint)(l)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort HIWORD(nuint l) => ((ushort)((((nuint)(l)) >> 16) & 0xffff));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort HIWORD(nint l) => unchecked(HIWORD((nuint)(l)));

    public static int GET_X_LPARAM(LPARAM lp) => ((int)(short)LOWORD(lp));

    public static int GET_Y_LPARAM(LPARAM lp) => ((int)(short)HIWORD(lp));
}
