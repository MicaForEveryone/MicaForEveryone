using System;
using System.Text;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32
{
    public class Application
    {
        private const uint APPMODEL_ERROR_NO_PACKAGE = 15700;

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

        private Window _mainWindow;

        /// <summary>
        /// Run main loop with given main window
        /// </summary>
        public void Run(Window window)
        {
            _mainWindow = window;

            BeforeRun?.Invoke(this, EventArgs.Empty);

            _mainWindow.Destroy += Window_OnDestroy;

            while (NativeMethods.GetMessageW(out var msg, IntPtr.Zero, 0, 0))
            {
                var processed = false;
                BeforeTranslateMessage?.Invoke(window, ref msg, ref processed);
                if (processed) continue;
                NativeMethods.TranslateMessage(msg);
                NativeMethods.DispatchMessageW(msg);
            }

            _mainWindow.Destroy -= Window_OnDestroy;

            BeforeExit?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Post WM_QUIT to stop main loop
        /// </summary>
        public void Exit()
        {
            NativeMethods.PostQuitMessage(0);
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
