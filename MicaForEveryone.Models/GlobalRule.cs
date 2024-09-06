using MicaForEveryone.PInvoke;

namespace MicaForEveryone.Models;

public sealed partial class GlobalRule : Rule
{
    public override bool Equals(Rule? other)
    {
        return other is not null 
            && other is GlobalRule
            && base.Equals(other);
    }

    public override bool IsRuleApplicable(Windowing.HWND hWnd) => true;
}
