using System;
using Vanara.PInvoke;

using MicaForEveryone.Win32;

namespace MicaForEveryone
{
    public class Application
    {
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.ShowErrorTaskDialog("Error: " + (e.ExceptionObject as Exception)?.Message,
                e.ExceptionObject.ToString());
        }

        public void Run(Window window)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            BeforeRun?.Invoke(this, EventArgs.Empty);

            window.Destroy += Window_OnDestroy;
            window.Activate();

            while (User32.GetMessage(out var msg, HWND.NULL, 0, 0))
            {
                var processed = false;
                BeforeTranslateMessage?.Invoke(window, ref msg, ref processed);
                if (processed) continue;
                User32.TranslateMessage(msg);
                User32.DispatchMessage(msg);
            }

            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
        }

        private void Window_OnDestroy(object sender, EventArgs e)
        {
            User32.PostQuitMessage();
        }

        public event EventHandler BeforeRun;
        public event MessageLoopHandler BeforeTranslateMessage;
    }

    public delegate void MessageLoopHandler(Window window, ref MSG message, ref bool processed);
}
