using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Generic;

namespace MicaForEveryone.PInvoke;

public static unsafe partial class Monitor
{
    public readonly struct HMONITOR : IEquatable<HMONITOR>
    {
        public readonly void* Value;

        public HMONITOR(void* value)
        {
            Value = value;
        }

        public static HMONITOR NULL => new HMONITOR(null);

        public static bool operator ==(HMONITOR left, HMONITOR right) => left.Value == right.Value;

        public static bool operator !=(HMONITOR left, HMONITOR right) => left.Value != right.Value;

        public override bool Equals(object? obj) => (obj is HMONITOR other) && Equals(other);

        public bool Equals(HMONITOR other) => ((nuint)(Value)).Equals((nuint)(other.Value));

        public override int GetHashCode() => ((nuint)(Value)).GetHashCode();
    }

    public struct MONITORINFO
    {
        public uint cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern HMONITOR MonitorFromRect(RECT* lprc, uint dwFlags);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMonitorInfoW(HMONITOR hMonitor, MONITORINFO* lpmi);

    public const uint MONITOR_DEFAULTTONULL = 0x00000000;
}
