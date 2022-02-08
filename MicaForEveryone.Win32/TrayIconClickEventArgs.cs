using System;
using System.Drawing;

namespace MicaForEveryone.Win32
{
    public class TrayIconClickEventArgs : EventArgs
    {
        public TrayIconClickEventArgs(Point point)
        {
            Point = point;
        }

        public Point Point { get; }
    }
}
