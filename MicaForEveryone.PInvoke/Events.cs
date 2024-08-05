using System.Runtime.InteropServices;
using static MicaForEveryone.PInvoke.Modules;
using static MicaForEveryone.PInvoke.Windowing;

namespace MicaForEveryone.PInvoke;

public static unsafe class Events
{
    [DllImport("user32", ExactSpelling = true)]
    public static extern HWINEVENTHOOK SetWinEventHook(uint eventMin, uint eventMax, HINSTANCE hmodWinEventProc, delegate* unmanaged<HWINEVENTHOOK, uint, HWND, int, int, uint, uint, void> pfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    public readonly struct HWINEVENTHOOK
    {
        public readonly void* Value;

        public HWINEVENTHOOK(void* value)
        {
            Value = value;
        }
    }

    public const uint EVENT_OBJECT_SHOW = 32770;
    public const uint OBJID_WINDOW = 0;
    public const uint WINEVENT_OUTOFCONTEXT = 0;
}