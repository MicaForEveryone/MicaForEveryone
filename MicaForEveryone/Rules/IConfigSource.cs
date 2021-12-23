using System.Collections.Generic;

namespace MicaForEveryone.Rules
{
    public interface IConfigSource
    {
        GlobalRule GetGlobalRule();
        IEnumerable<IRule> ParseRules();
        void Reload();
        void Save(GlobalRule rule);
    }
}
