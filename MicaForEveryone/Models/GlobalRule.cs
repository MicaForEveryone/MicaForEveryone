using Vanara.PInvoke;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.Models
{
    public class GlobalRule : IRule
    {
        public string Name => "Global";

        public TitlebarColorMode TitlebarColor { get; set; }

        public BackdropType BackdropPreference { get; set; }

        public bool ExtendFrameIntoClientArea { get; set; }

        public bool IsApplicable(HWND windowHandle) => windowHandle.HasCaption();

        public override string ToString() => Name;
    }
}
