using System;
using System.Runtime.InteropServices;

namespace MicaForEveryone.Models
{
    [ComVisible(true)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFocusableWindow
    {
        event EventHandler GotFocus;
        event EventHandler LostFocus;
    }
}
