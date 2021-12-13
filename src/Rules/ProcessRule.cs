using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class ProcessRule : IRule
    {
        public ProcessRule(string processName, TitlebarColorMode titlebarColor, MicaPreference mica)
        {
            ProcessName = processName;
            TitlebarColor = titlebarColor;
            MicaPreference = mica;
        }

        public string ProcessName { get; }

        public TitlebarColorMode TitlebarColor { get; }

        public MicaPreference MicaPreference { get; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetProcessName() == ProcessName;
        }

        public override string ToString() => $"Rule: Process={ProcessName}";
    }
}
