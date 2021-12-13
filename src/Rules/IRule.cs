using Vanara.PInvoke;

namespace MicaForEveryone.Rules
{
    public interface IRule
    {
        bool IsApplicable(HWND windowHandle);
        ThemeMode Theme { get; }
        MicaMode Mica { get; }
    }

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
}
