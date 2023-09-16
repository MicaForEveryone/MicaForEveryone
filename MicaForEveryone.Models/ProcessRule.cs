namespace MicaForEveryone.Models;

public class ProcessRule : IRule
{
    public string Name => $"Process ({ProcessName})";

    public required string ProcessName { get; set; }

    public TitleBarColorMode TitleBarColor { get; set; }

    public BackdropType BackdropPreference { get; set; }

    public CornerPreference CornerPreference { get; set; }

    public bool ExtendFrameIntoClientArea { get; set; }

    public bool EnableBlurBehind { get; set; }

    public string? TitleBarColorCode { get; set; }

    public bool Equals(IRule? other)
    {
        return other is not null
            && other is ProcessRule processRule
            && ProcessName.Equals(processRule.ProcessName, StringComparison.CurrentCultureIgnoreCase)
            && TitleBarColor == processRule.TitleBarColor
            && BackdropPreference == processRule.BackdropPreference
            && CornerPreference == processRule.CornerPreference
            && ExtendFrameIntoClientArea == processRule.ExtendFrameIntoClientArea
            && EnableBlurBehind == processRule.EnableBlurBehind
            && TitleBarColorCode == processRule.TitleBarColorCode;
    }
}