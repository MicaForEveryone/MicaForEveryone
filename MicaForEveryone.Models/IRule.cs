namespace MicaForEveryone.Models;

public interface IRule
{
    string Name { get; }

    const int Priority = 0;

    TitleBarColorMode TitleBarColor { get; set; }

    BackdropType BackdropPreference { get; set; }

    CornerPreference CornerPreference { get; set; }

    bool ExtendFrameIntoClientArea { get; set; }

    bool EnableBlurBehind { get; set; }

    string TitleBarColorCode { get; set; }
}
