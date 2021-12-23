using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using MicaForEveryone.Extensions;

namespace MicaForEveryone.Rules
{
    public class ConfigFile : IConfigSource
    {
        private readonly string _filePath;
        private IniData _data;

        public ConfigFile(string path)
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

        public void Save(GlobalRule rule)
        {
            _data.Global["TitleBarColor"] = rule.TitlebarColor.ToString();
            _data.Global["BackdropPreference"] = rule.BackdropPreference.ToString();
            _data.Global["ExtendFrameIntoClientArea"] = rule.ExtendFrameIntoClientArea.ToString();
            new FileIniDataParser().WriteFile(_filePath, _data);
        }
    }
}
