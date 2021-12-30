using System.Collections.Generic;

namespace MicaForEveryone.Rules
{
    public interface IConfigSource
    {
        IEnumerable<IRule> ParseRules();
        void OverrideRule(IRule rule);
    }
}
