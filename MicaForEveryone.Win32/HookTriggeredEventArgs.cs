using System;
using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public class HookTriggeredEventArgs : EventArgs
    {
        public uint Event { get; set; }
        public HWND WindowHandle { get; set; }
        public int ObjectId { get; set; }
        public int ChildId { get; set; }
    }
}
