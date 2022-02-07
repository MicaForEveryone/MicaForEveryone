using System;

using MicaForEveryone.Win32.PInvoke;
using static MicaForEveryone.Win32.PInvoke.NativeMethods;

namespace MicaForEveryone.Win32
{
    public class Application
    {
        private Window _mainWindow;
        
        /// <summary>
        /// Run main loop with given main window
        /// </summary>
        public void Run(Window window)
        {
            _mainWindow = window;

            BeforeRun?.Invoke(this, EventArgs.Empty);

            _mainWindow.Destroy += Window_OnDestroy;

            while (GetMessageW(out var msg, IntPtr.Zero, 0, 0))
            {
                var processed = false;
                BeforeTranslateMessage?.Invoke(window, ref msg, ref processed);
                if (processed) continue;
                TranslateMessage(msg);
                DispatchMessageW(msg);
            }

            _mainWindow.Destroy -= Window_OnDestroy;

            BeforeExit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Post WM_QUIT to stop main loop
        /// </summary>
        public void Exit()
        {
            PostQuitMessage(0);
        }

        private void Window_OnDestroy(object sender, EventArgs e)
        {
            // stop main loop when window destroyed
            Exit();
        }

        public event EventHandler BeforeRun;
        public event MessageLoopHandler BeforeTranslateMessage;
        public event EventHandler BeforeExit;
    }

    public delegate void MessageLoopHandler(Window window, ref MSG message, ref bool processed);
}
