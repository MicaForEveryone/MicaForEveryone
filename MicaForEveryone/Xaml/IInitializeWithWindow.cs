using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Xaml
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
    public interface IInitializeWithWindow
    {
        [PreserveSig]
        IntPtr Initialize(IntPtr hWnd);
    }
}