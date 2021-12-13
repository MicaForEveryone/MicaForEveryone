using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
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
