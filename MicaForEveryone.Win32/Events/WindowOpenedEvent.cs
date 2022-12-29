using System;

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

            if (window == null)
                return;

            Handler?.Invoke(this, new WindowOpenedEventArgs(window));
        }

        public event EventHandler<WindowOpenedEventArgs> Handler;
    }
}
