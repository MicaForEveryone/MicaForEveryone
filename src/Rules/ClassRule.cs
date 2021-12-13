using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class ClassRule : IRule
    {
        public ClassRule(string className, TitlebarColorMode titlebarColor, MicaPreference mica)
        {
            ClassName = className;
            TitlebarColor = titlebarColor;
            MicaPreference = mica;
        }

        public string ClassName { get; }

        public TitlebarColorMode TitlebarColor { get; }

        public MicaPreference MicaPreference { get; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetClassName() == ClassName;
        }

        public override string ToString() => $"Rule: ClassName={ClassName}";
    }
}
