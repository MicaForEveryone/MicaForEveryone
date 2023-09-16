using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicaForEveryone.Models;

public class ClassRule : IRule
{
    public string Name => $"Window class ({ClassName})";

    public required string ClassName { get; set; }

    public TitleBarColorMode TitleBarColor { get; set; }

    public BackdropType BackdropPreference { get; set; }

    public CornerPreference CornerPreference { get; set; }

    public bool ExtendFrameIntoClientArea { get; set; }

    public bool EnableBlurBehind { get; set; }

    public string? TitleBarColorCode { get; set; }

    public bool Equals(IRule? other)
    {
        return other is not null && other is ClassRule classRule && ClassName.Equals(classRule.ClassName, StringComparison.CurrentCultureIgnoreCase);
    }
}