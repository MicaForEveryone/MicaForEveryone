using System;
using System.Collections.Generic;
using System.Text;

namespace MicaForEveryone.Win32
{
    public static class SystemBackdrop
    {
        public static bool IsSupported => Environment.OSVersion.Version.Build >= 22523;
    }
}
