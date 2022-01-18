using Vanara.PInvoke;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.Models
{
    public class ProcessRule : IRule
    {
        public ProcessRule(string processName)
        {
            ProcessName = processName;
        }

        public string Name => $"Process({ProcessName})";

        public string ProcessName { get; }

        public TitlebarColorMode TitlebarColor { get; set; }

        public BackdropType BackdropPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetProcessName() == ProcessName;
        }

        public override string ToString() => Name;
    }
}
