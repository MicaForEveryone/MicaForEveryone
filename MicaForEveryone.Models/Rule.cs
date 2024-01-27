using CommunityToolkit.Mvvm.ComponentModel;

namespace MicaForEveryone.Models;

public abstract partial class Rule: ObservableObject, IEquatable<Rule>
{
    TitleBarColorMode _titleBarColor;
    BackdropType _backdropPreference;
    CornerPreference _cornerPreference;
    bool _extendFrameIntoClientArea;
    bool _enableBlurBehind;
    string? _titleBarColorCode;

    public TitleBarColorMode TitleBarColor
    {
        get => _titleBarColor;
        set => SetProperty(ref _titleBarColor, value);
    }

    public BackdropType BackdropPreference
    {
        get => _backdropPreference;
        set => SetProperty(ref _backdropPreference, value);
    }

    public CornerPreference CornerPreference
    {
        get => _cornerPreference;
        set => SetProperty(ref _cornerPreference, value);
    }

    public bool ExtendFrameIntoClientArea
    {
        get => _extendFrameIntoClientArea;
        set => SetProperty(ref _extendFrameIntoClientArea, value);
    }

    public bool EnableBlurBehind
    {
        get => _enableBlurBehind;
        set => SetProperty(ref _enableBlurBehind, value);
    }

    public string? TitleBarColorCode
    {
        get => _titleBarColorCode;
        set => SetProperty(ref _titleBarColorCode, value);
    }

    public virtual bool Equals(Rule? other)
    {
        return TitleBarColor == other!.TitleBarColor
            && BackdropPreference == other.BackdropPreference
            && CornerPreference == other.CornerPreference
            && ExtendFrameIntoClientArea == other.ExtendFrameIntoClientArea
            && EnableBlurBehind == other.EnableBlurBehind
            && TitleBarColorCode == other.TitleBarColorCode;
    }
}
