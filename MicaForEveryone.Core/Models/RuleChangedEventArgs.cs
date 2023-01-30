using MicaForEveryone.Core.Interfaces;

namespace MicaForEveryone.Core.Models
{
    public class RulesChangeEventArgs : EventArgs
    {
        public RulesChangeEventArgs(IRule rule)
        {
            Rule = rule;
        }

        public IRule Rule { get; }
    }
}
