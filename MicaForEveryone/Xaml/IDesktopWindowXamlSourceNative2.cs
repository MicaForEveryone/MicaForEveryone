using System;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Xaml
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("e3dcd8c7-3057-4692-99c3-7b7720afda31")]
    public interface IDesktopWindowXamlSourceNative2
    {
        void AttachToWindow(IntPtr parentWnd);
        IntPtr WindowHandle { get; }
        bool PreTranslateMessage(ref MSG message);
    }
}