using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public interface IWindow
    {
        public HWND Handle { get; }
    }
}
