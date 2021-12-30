using Vanara.PInvoke;

using MicaForEveryone.Extensions;
using MicaForEveryone.Models;

namespace MicaForEveryone.Rules
{
    public class ClassRule : IRule
    {
        public ClassRule(string className)
        {
            ClassName = className;
        }

        public string Name => $"Class({ClassName})";

        public string ClassName { get; }

        public TitlebarColorMode TitlebarColor { get; set; }

        public BackdropType BackdropPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle)
        {
            return windowHandle.GetClassName() == ClassName;
        }

        public override string ToString() => Name;
    }
}
