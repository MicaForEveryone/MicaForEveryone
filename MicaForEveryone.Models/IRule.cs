using System.Text.Json.Serialization;

namespace MicaForEveryone.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "scope")]
[JsonDerivedType(typeof(ProcessRule), "process")]
[JsonDerivedType(typeof(GlobalRule), "global")]
[JsonDerivedType(typeof(ClassRule), "windowClass")]
public abstract class Rule : IEquatable<Rule>
{
    public abstract string Name { get; }

    public TitleBarColorMode TitleBarColor { get; set; }

    public BackdropType BackdropPreference { get; set; }

    public CornerPreference CornerPreference { get; set; }

    public bool ExtendFrameIntoClientArea { get; set; }

    public bool EnableBlurBehind { get; set; }

    public string? TitleBarColorCode { get; set; }

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
