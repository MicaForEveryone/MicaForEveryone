using System;
using System.Threading;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32.Events
{
	/// <summary>
	/// Event for new window opened
	/// </summary>
	public class WindowOpenedEvent : WinEvent
    {
        public WindowOpenedEvent() : base(WinEventType.ObjectShown)
        {
        }

        protected override void EventCallback(WinEventArgs args)
        {
            if (args.ObjectId != ObjectIdentifiers.OBJID_WINDOW)
                return;

            var window = Window.GetWindowIfWindowPatternValid(args.WindowHandle);
            
            // Wait for a while, it may get a valid window
            var count = 0;
			while (count < 10 && window == null)
			{
                Thread.Sleep(10);

				window = Window.GetWindowIfWindowPatternValid(args.WindowHandle);

				count++;
			}

            if (window == null) return;

			Handler?.Invoke(this, new WindowOpenedEventArgs(window));
        }

        public event EventHandler<WindowOpenedEventArgs> Handler;
    }
}
