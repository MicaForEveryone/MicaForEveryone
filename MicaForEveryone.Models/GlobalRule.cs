using System.Text.Json.Serialization;

namespace MicaForEveryone.Models;

public class GlobalRule : Rule
{
    public override string Name => "Global";

    public override bool Equals(Rule? other)
    {
        return other is not null 
            && other is GlobalRule
            && base.Equals(other);
    }
}
