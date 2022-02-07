using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32
{
    public class WindowClass : IDisposable
    {
        private const int GCW_ATOM = -32;
        private const int GCL_HMODULE = -16;

        /// <summary>
        /// Get window class of given window
        /// </summary>
        public static WindowClass GetClassOfWindow(IntPtr hWnd)
        {
            var atom = Macros.MAKEINTATOM(NativeMethods.GetClassWord(hWnd, GCW_ATOM));
            var module = NativeMethods.GetClassLongPtrW(hWnd, GCL_HMODULE);
            var className = NativeMethods.GetClassName(hWnd);
            var classData = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                hInstance = module,
                lpszClassName = className,
            };

            // FIXME: these don't work, idk why
            //
            //if (!GetClassInfoExW(module, className, ref classData))
            //{
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
            //}
            //
            //if (!GetClassInfoExW(module, atom, ref classData))
            //{
            //    throw new Win32Exception(Marshal.GetLastWin32Error());
            //}

            return new WindowClass(atom, classData);
        }

        private readonly WNDCLASSEX _classData;
        
        /// <summary>
        /// Create and register a window class
        /// </summary>
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
            Atom = Macros.MAKEINTATOM(NativeMethods.RegisterClassExW(_classData));
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

        /// <summary>
        /// Unregister window class
        /// </summary>
        public void Dispose()
        {
            if (!NativeMethods.UnregisterClassW(Name, Application.InstanceHandle))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            Atom = IntPtr.Zero;
        }
    }
}
