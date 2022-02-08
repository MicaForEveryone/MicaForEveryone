
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Win32.Events
{
    public enum WinEventType : uint
    {
        ObjectCreate = EventConstants.EVENT_OBJECT_CREATE,
        ObjectShown = EventConstants.EVENT_OBJECT_SHOW,
    }
}
