using Vanara.PInvoke;

namespace MicaForEveryone.Rules
{
    public interface IRule
    {
        bool IsApplicable(HWND windowHandle);
        TitlebarColorMode TitlebarColor { get; set; }
        MicaPreference MicaPreference { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }
    }

    public enum TitlebarColorMode
    {
        Default = 0,
        Light = 1,
        Dark = 2,
    }

    public enum MicaPreference
    {
        Default = 0,
        PreferEnabled = 1,
        PreferDisabled = 3,
    }
}
