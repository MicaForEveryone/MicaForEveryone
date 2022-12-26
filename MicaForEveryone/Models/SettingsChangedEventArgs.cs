using System;

using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Models
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
