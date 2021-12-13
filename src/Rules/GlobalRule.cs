using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class GlobalRule : IRule
    {
        public GlobalRule(TitlebarColorMode titlebarColor, MicaPreference mica, bool extendFrameIntoClientArea)
        {
            TitlebarColor = titlebarColor;
            MicaPreference = mica;
            ExtendFrameIntoClientArea = extendFrameIntoClientArea;
        }

        public TitlebarColorMode TitlebarColor { get; set; }

        public MicaPreference MicaPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle) => windowHandle.HasCaption();

        public override string ToString() => "Rule: Global";
    }
}
