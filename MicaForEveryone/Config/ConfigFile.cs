using System;
using System.Collections.Generic;

using MicaForEveryone.Rules;

namespace MicaForEveryone.Config
{
    public class ConfigFile : IConfigSource
    {
        private readonly string _filePath;

        public ConfigFile(string path)
        {
            _filePath = path;
        }

        public IEnumerable<IRule> ParseRules()
        {
            return Parser.ParseFile(_filePath);
        }

        public void OverrideRule(IRule rule)
        {
            throw new NotImplementedException();
        }
    }
}
