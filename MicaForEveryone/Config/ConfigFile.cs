using System.Collections.Generic;
using System.IO;

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

        public void ReadFile()
        {
            
        }

        public IEnumerable<IRule> ParseRules()
        {
            return null;
        }

        public GlobalRule GetGlobalRule()
        {
            var lexer = new Lexer(File.ReadAllLines(_filePath));
            var tokenizer = new Tokenizer(lexer.Parse());
            var tokens = tokenizer.Parse();
            return null;
        }

        public void Reload()
        {
            ReadFile();
        }

        public void Save(GlobalRule rule)
        {
        }
    }
}
