using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public class WindowClass : IDisposable
    {
        private readonly WNDCLASSEX _classData;
        
        public WindowClass(IntPtr module, string name, WndProc wndProc, IntPtr icon, WindowClassStyles styles = 0, int wndExtra = 0)
        {
            _classData = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                lpfnWndProc = wndProc,
                hInstance = module,
                lpszClassName = name,
                style = styles,
                hIcon = icon,
                hIconSm = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                cbClsExtra = 0,
                cbWndExtra = wndExtra,
            };
            Atom = Macros.MAKEINTATOM(RegisterClassExW(_classData));
            if (Atom == IntPtr.Zero)
            {
                var error = Marshal.GetLastWin32Error();
                throw new Win32Exception(error);
            }
        }

        public string Name => _classData.lpszClassName;

        public IntPtr Icon => _classData.hIcon;

        public IntPtr Atom { get; private set; }

        public void Dispose()
        {
            if (!UnregisterClassW(Name, InstanceHandle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            Atom = IntPtr.Zero;
        }
    }
}
