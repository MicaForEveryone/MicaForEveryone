using System;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace MicaForEveryone.Xaml
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("e3dcd8c7-3057-4692-99c3-7b7720afda31")]
    public interface IDesktopWindowXamlSourceNative2
    {
        void AttachToWindow(HWND parentWnd);
        HWND WindowHandle { get; }
        bool PreTranslateMessage(ref MSG message);
    }
}