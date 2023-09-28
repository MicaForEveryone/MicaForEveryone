using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace MicaForEveryone.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "scope")]
[JsonDerivedType(typeof(ProcessRule), "process")]
[JsonDerivedType(typeof(GlobalRule), "global")]
[JsonDerivedType(typeof(ClassRule), "windowClass")]
public abstract partial class Rule: ObservableObject, IEquatable<Rule>
{
    [ObservableProperty]
    TitleBarColorMode _titleBarColor;

    [ObservableProperty]
    BackdropType _backdropPreference;

    [ObservableProperty]
    CornerPreference _cornerPreference;

    [ObservableProperty]
    bool _extendFrameIntoClientArea;

    [ObservableProperty]
    bool _enableBlurBehind;

    [ObservableProperty]
    string? _titleBarColorCode;

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
