using System;

namespace MicaForEveryone.Win32
{
    public class WndProcEventArgs : EventArgs
    {
        public WndProcEventArgs(IntPtr windowHandle)
        {
            WindowHandle = windowHandle;
        }

        public IntPtr WindowHandle { get; }
    }
}
