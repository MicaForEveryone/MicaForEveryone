using System;
using System.Collections.Generic;
using System.Text;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32
{
    public class Application
    {
        private const uint APPMODEL_ERROR_NO_PACKAGE = 15700;

        public static IntPtr InstanceHandle { get; } = NativeMethods.GetCurrentModule();

        public static bool IsPackaged { get; } = GetCurrentPackageName() != null;

        public static string GetCurrentPackageName()
        {
            var length = 0u;
            NativeMethods.GetCurrentPackageFullName(ref length);

            var result = new StringBuilder((int)length);
            var error = NativeMethods.GetCurrentPackageFullName(ref length, result);

            if (error == APPMODEL_ERROR_NO_PACKAGE)
                return null;

            return result.ToString();
        }
        
        private readonly List<Window> _windows = new();
        
        public Dispatcher Dispatcher { get; } = new();

        public void AddWindow(Window window)
        {
            _windows.Add(window);
            window.Destroy += (_, _) =>
            {
                _windows.Remove(window);
            };
        }
        
        /// <summary>
        /// Run main loop
        /// </summary>
        public void Run()
        {
            BeforeRun?.Invoke(this, EventArgs.Empty);
            
            while (NativeMethods.GetMessageW(out var msg, IntPtr.Zero, 0, 0))
            {
                var processed = false;
                foreach (var window in _windows)
                {
                    BeforeTranslateMessage?.Invoke(window, ref msg, ref processed);
                    if (processed) break;
                }
                if (processed) continue;
                NativeMethods.TranslateMessage(msg);
                NativeMethods.DispatchMessageW(msg);
                Dispatcher.Invoke();
            }
            
            BeforeExit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Post WM_QUIT to stop main loop
        /// </summary>
        public void Exit()
        {
            NativeMethods.PostQuitMessage(0);
        }
        
        public event EventHandler BeforeRun;
        public event MessageLoopHandler BeforeTranslateMessage;
        public event EventHandler BeforeExit;
    }

    public delegate void MessageLoopHandler(Window window, ref MSG message, ref bool processed);
}
