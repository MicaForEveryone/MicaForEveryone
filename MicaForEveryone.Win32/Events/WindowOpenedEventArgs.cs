using System;

namespace MicaForEveryone.Win32.Events
{
	public class WindowOpenedEventArgs : EventArgs
    {
        public Window Window { get; }

		public WindowOpenedEventArgs(Window window)
		{
			Window = window;
		}
	}
}
