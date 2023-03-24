using System;

namespace MicaForEveryone.Win32.Events
{
    public class WinEventArgs : EventArgs
    {
        public WinEventType EventId { get; set; }
        public IntPtr WindowHandle { get; set; }
        public int ObjectId { get; set; }
        public int ChildId { get; set; }
        public uint EventTime { get; set; }
    }
}
