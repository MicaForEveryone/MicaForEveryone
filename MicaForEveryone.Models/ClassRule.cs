namespace MicaForEveryone.Models;

public sealed class ClassRule : Rule
{
    public required string ClassName { get; set; }

    public override bool Equals(Rule? other)
    {
        return other is not null
            && other is ClassRule cRule
            && ClassName.Equals(cRule.ClassName, StringComparison.CurrentCultureIgnoreCase)
            && base.Equals(other);
    }
}