using Vanara.PInvoke;

namespace MicaForEveryone
{
    public enum ThemeMode
    {
        Default = 0,
        ForceLight = 1,
        ForceDark = 2,
    }

    public enum MicaMode
    {
        Default = 0,
        ForceMica = 1,
        ForceNoMica = 2,
    }

    public interface IRule
    {
        bool IsApplicable(HWND windowHandle);
        ThemeMode Theme { get; }
        MicaMode Mica { get; }
    }

    public class GlobalRule : IRule
    {
        public GlobalRule(ThemeMode theme, MicaMode mica)
        {
            Theme = theme;
            Mica = mica;
        }

        public ThemeMode Theme { get; set; }

        public MicaMode Mica { get; set; }

        public bool IsApplicable(HWND windowHandle) => windowHandle.HasCaption();

        public override string ToString() => $"Rule: Global";
    }

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

    public class ClassRule : IRule
    {
        public ClassRule(string className, ThemeMode theme, MicaMode mica)
        {
            ClassName = className;
            Theme = theme;
            Mica = mica;
        }

        public string ClassName { get; }

        public ThemeMode Theme { get; }

        public MicaMode Mica { get; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetClassName() == ClassName;
        }

        public override string ToString() => $"Rule: ClassName={ClassName}";
    }
}
