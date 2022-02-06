using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public class WindowClass : IDisposable
    {
        private const int GCW_ATOM = -32;
        private const int GCL_HMODULE = -16;

        public static WindowClass GetClassOfWindow(IntPtr hWnd)
        {
            var atom = Macros.MAKEINTATOM(GetClassWord(hWnd, GCW_ATOM));
            var module = GetClassLongPtrW(hWnd, GCL_HMODULE);
            var className = GetClassName(hWnd);
            var classData = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                hInstance = module,
                lpszClassName = className,
            };
            //if (!GetClassInfoExW(module, className, ref classData))
            //{
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
            //}
            return new WindowClass(atom, classData);
        }

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

        private WindowClass(IntPtr atom, WNDCLASSEX classData)
        {
            Atom = atom;
            _classData = classData;
        }

        public string Name => _classData.lpszClassName;

        public IntPtr Icon => _classData.hIcon;

        public IntPtr Module => _classData.hInstance;

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
