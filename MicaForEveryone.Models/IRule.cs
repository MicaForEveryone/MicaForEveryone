using System.Text.Json.Serialization;

namespace MicaForEveryone.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "scope")]
[JsonDerivedType(typeof(ProcessRule), "process")]
[JsonDerivedType(typeof(GlobalRule), "global")]
[JsonDerivedType(typeof(ClassRule), "windowClass")]
public interface IRule : IEquatable<IRule>
{
    string Name { get; }

    TitleBarColorMode TitleBarColor { get; set; }

    BackdropType BackdropPreference { get; set; }

    CornerPreference CornerPreference { get; set; }

    bool ExtendFrameIntoClientArea { get; set; }

    bool EnableBlurBehind { get; set; }

    string? TitleBarColorCode { get; set; }
}
