using System.Collections.Generic;

namespace MicaForEveryone.Interfaces
{
    public interface IConfigSource
    {
        IEnumerable<IRule> ParseRules();
        void OverrideRule(IRule rule);
    }
}
