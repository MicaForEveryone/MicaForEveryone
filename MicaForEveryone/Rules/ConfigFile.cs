using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using MicaForEveryone.Config;

namespace MicaForEveryone.Rules
{
    public class ConfigFile : IConfigSource
    {
        private readonly string _filePath;

        private Document _configDocument;

        public ConfigFile(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<IRule> ParseRules()
        {
            var lines = File.ReadAllLines(_filePath);
            _configDocument = Document.Parse(lines);
            return _configDocument.ToRules();
        }

        public void OverrideRule(IRule rule)
        {
            if (rule is not GlobalRule)
                throw new NotImplementedException();
            var target = _configDocument.Sections.First(
                section => section.Type.Value == SectionType.Global);
            target.OverrideSection(rule);
            File.WriteAllText(_filePath, _configDocument.ToString());
        }
    }
}
