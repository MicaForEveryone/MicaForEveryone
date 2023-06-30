using System.Runtime.InteropServices;

namespace MicaForEveryone.Core.Ui.Interfaces;

[ComVisible(true)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IFocusableWindow
{
    event EventHandler GotFocus;
    event EventHandler LostFocus;
}