using System;
using System.Collections.Generic;

using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32.Events
{
    public abstract class WinEvent
    {
        private readonly WinEventProc _callback;
        private readonly Queue<WinEventArgs> _callbacksWaitList = new();

        private IntPtr _handle = IntPtr.Zero;
        private bool _busy = false;

        protected WinEvent(WinEventType type)
        {
            EventType = type;
            _callback = new WinEventProc(QueueEventCallback);
        }

        public WinEventType EventType { get; }

        internal void StartListening()
        {
            _handle = NativeMethods.SetWinEventHook((uint)EventType, (uint)EventType, Application.InstanceHandle, _callback, 0, 0, WINEVENT.WINEVENT_OUTOFCONTEXT);
        }

        internal void StopListening()
        {
            if (_handle == IntPtr.Zero)
                return;
            NativeMethods.UnhookWinEvent(_handle);
            _handle = IntPtr.Zero;
        }

        protected abstract void EventCallback(WinEventArgs args);

        private void QueueEventCallback(IntPtr hWinEventHook, uint winEvent, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime)
        {
            _callbacksWaitList.Enqueue(new WinEventArgs
            {
                EventId = (WinEventType)winEvent,
                WindowHandle = hwnd,
                ObjectId = idObject,
                ChildId = idChild,
                EventTime = dwmsEventTime,
            });

            if (_busy) return;

            _busy = true;

            while (_callbacksWaitList.Count > 0)
            {
                var args = _callbacksWaitList.Dequeue();
                try
                {
                    if (args.ObjectId > 0 && NativeMethods.UiaHasServerSideProvider(args.WindowHandle))
                    {
                        // Ignore events from the UIA->MSAA bridge
                        // Check MS.Internal.Automation.WinEventWrap in https://github.com/dotnet/wpf
                        continue;
                    }

                    if (args.EventTime == 0)
                    {
                        args.EventTime = 1;
                    }

                    EventCallback(args);
                }
#if DEBUG
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
#else
                    catch
                    {
                        // ignore
                    }
#endif
            }

            _busy = false;
        }
    }
}
