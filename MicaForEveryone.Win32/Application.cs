using System;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

namespace MicaForEveryone.Win32
{
    public class Application
    {
        public void Run(NativeWindow window)
        {
            BeforeRun?.Invoke(this, EventArgs.Empty);

            window.Destroy += Window_OnDestroy;

            while (GetMessage(out var msg, HWND.NULL, 0, 0))
            {
                var processed = false;
                BeforeTranslateMessage?.Invoke(window, ref msg, ref processed);
                if (processed) continue;
                TranslateMessage(msg);
                DispatchMessage(msg);
            }

            BeforeExit?.Invoke(this, EventArgs.Empty);
        }

        public void Exit()
        {
            PostQuitMessage();
        }

        private void Window_OnDestroy(object sender, EventArgs e)
        {
            Exit();
        }

        public event EventHandler BeforeRun;
        public event MessageLoopHandler BeforeTranslateMessage;
        public event EventHandler BeforeExit;
    }

    public delegate void MessageLoopHandler(NativeWindow window, ref MSG message, ref bool processed);
}
