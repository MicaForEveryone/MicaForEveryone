namespace MicaForEveryone.Models;

public sealed class ProcessRule : Rule
{
    public required string ProcessName { get; set; }

    public override bool Equals(Rule? other)
    {
        return other is not null 
            && other is ProcessRule pRule 
            && ProcessName.Equals(pRule.ProcessName, StringComparison.CurrentCultureIgnoreCase) 
            && base.Equals(other);
    }
}