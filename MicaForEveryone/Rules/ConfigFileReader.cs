using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class ConfigFileReader : IConfigSource
    {
        private readonly string _filePath;
        private IniData _data;

        public ConfigFileReader(string path)
        {
            _filePath = path;
        }

        public void ReadFile()
        {
            var parser = new FileIniDataParser();
            _data = parser.ReadFile(_filePath);
        }

        public IEnumerable<IRule> ParseRules()
        {
            if (_data == null)
            {
                ReadFile();
            }

            foreach (var iniRule in _data.Sections)
            {
                yield return iniRule.ParseRule();
            }
        }

        public GlobalRule GetGlobalRule()
        {
            if (_data == null)
            {
                ReadFile();
            }

            var result = new GlobalRule();
            var rule = (IRule) result;
            _data.Global.ParseRule(ref rule);
            return result;
        }

        public void Reload()
        {
            _data = null;
            ReadFile();
        }
    }
}
