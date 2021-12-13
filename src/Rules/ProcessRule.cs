using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class ProcessRule : IRule
    {
        public ProcessRule(string processName, ThemeMode theme, MicaMode mica)
        {
            ProcessName = processName;
            Theme = theme;
            Mica = mica;
        }

        public string ProcessName { get; }

        public ThemeMode Theme { get; }

        public MicaMode Mica { get; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetProcessName() == ProcessName;
        }

        public override string ToString() => $"Rule: Process={ProcessName}";
    }
}
