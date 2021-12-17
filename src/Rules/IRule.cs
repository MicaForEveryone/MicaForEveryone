using Vanara.PInvoke;

namespace MicaForEveryone.Rules
{
    public interface IRule
    {
        bool IsApplicable(HWND windowHandle);
        TitlebarColorMode TitlebarColor { get; set; }
        BackdropType BackdropPreference { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }
    }

    public enum TitlebarColorMode
    {
        Default = 0,
        Light = 1,
        Dark = 2,
    }

    public enum BackdropType
    {
        Default = 0,
        None = 1,
        Mica = 2,
        Acrylic = 3,
        Tabbed = 4,
    }
}
