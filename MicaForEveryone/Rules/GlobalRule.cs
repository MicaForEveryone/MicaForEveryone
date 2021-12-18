using Vanara.PInvoke;

using MicaForEveryone.Extensions;
using MicaForEveryone.Models;

namespace MicaForEveryone.Rules
{
    public class GlobalRule : IRule
    {
        public TitlebarColorMode TitlebarColor { get; set; }

        public BackdropType BackdropPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle) => windowHandle.HasCaption();

        public override string ToString() => "Rule: Global";
    }
}
