namespace MicaForEveryone.Models;

public class GlobalRule : IRule
{
    public string Name => "Global";

    public TitleBarColorMode TitleBarColor { get; set; }

    public BackdropType BackdropPreference { get; set; }

    public CornerPreference CornerPreference { get; set; }

    public bool ExtendFrameIntoClientArea { get; set; }

    public bool EnableBlurBehind { get; set; }

    public string? TitleBarColorCode { get; set; }

    public bool Equals(IRule? other)
    {
        return other is not null 
            && other is GlobalRule
            && TitleBarColor == other.TitleBarColor
            && BackdropPreference == other.BackdropPreference
            && CornerPreference == other.CornerPreference
            && ExtendFrameIntoClientArea == other.ExtendFrameIntoClientArea
            && EnableBlurBehind == other.EnableBlurBehind
            && TitleBarColorCode == other.TitleBarColorCode;
    }
}
