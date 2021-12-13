using Vanara.PInvoke;

using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
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
}
