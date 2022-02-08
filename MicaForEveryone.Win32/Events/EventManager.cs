using System.Collections;

namespace MicaForEveryone.Win32.Events
{
    public static class WinEventManager
    {
        private static ArrayList _events = new(16);

        public static void AddEventHandler(WinEvent winEvent)
        {
            _events.Add(winEvent);
            winEvent.StartListening();
        }

        public static void RemoveEventHandler(WinEvent winEvent)
        {
            winEvent.StopListening();
            _events.Remove(winEvent);
        }

        public static void RemoveAll()
        {
            foreach (WinEvent winEvent in _events)
            {
                winEvent.StopListening();
            }
            _events.Clear();
        }
    }
}
