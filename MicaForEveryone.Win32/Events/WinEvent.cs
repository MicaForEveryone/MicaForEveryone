using System;
using System.Threading;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32.Events
{
	public abstract class WinEvent
	{
		private readonly WinEventProc _callback;

		private IntPtr _handle = IntPtr.Zero;

		protected WinEvent(WinEventType type)
		{
			EventType = type;
			_callback = new WinEventProc(EventCallbackWrapper);
		}

		public WinEventType EventType { get; }

		internal void StartListening()
		{
			_handle = NativeMethods.SetWinEventHook((uint)EventType, (uint)EventType, IntPtr.Zero, _callback, 0, 0, WINEVENT.WINEVENT_OUTOFCONTEXT);
		}

		internal void StopListening()
		{
			if (_handle == IntPtr.Zero)
				return;
			NativeMethods.UnhookWinEvent(_handle);
			_handle = IntPtr.Zero;
		}

		protected abstract void EventCallback(WinEventArgs args);

		private void EventCallbackWrapper(IntPtr hWinEventHook, uint winEvent, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
		{
			if (hwnd == IntPtr.Zero) return;
			
			try
			{
				var args = new WinEventArgs
				{
					EventId = (WinEventType)winEvent,
					WindowHandle = hwnd,
					ObjectId = idObject,
					ChildId = idChild,
					EventTime = dwmsEventTime,
				};

				if (args.ObjectId > 0 /*&& NativeMethods.UiaHasServerSideProvider(args.WindowHandle)*/)
				{
					// Ignore events from the UIA->MSAA bridge
					// Check MS.Internal.Automation.WinEventWrap in https://github.com/dotnet/wpf
					return;
				}

				// Why is this even needed?
				//if (args.EventTime == 0)
				//{
				//	args.EventTime = 1;
				//}

				EventCallback(args);
			}
#if DEBUG
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
				System.Diagnostics.Debugger.Break();
			}
#else
            catch
            {
                // ignore
            }
#endif
		}
	}
}
