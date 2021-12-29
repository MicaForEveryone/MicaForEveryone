using System.Collections.Generic;
using System.Linq;
using System.Text;

using MicaForEveryone.Rules;

namespace MicaForEveryone.Config
{
    public class Document
    {
        public static Document Parse(string[] lines)
        {
            var tokens = new Tokenizer(new Lexer(lines).Parse()).Parse();
            return new Parser(tokens).Parse();
        }

        private readonly Token[] _tokens;

        internal Document(Token[] tokens)
        {
            _tokens = tokens;
        }

        public IList<Section> Sections { get; } = new List<Section>();

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (var token in _tokens)
            {
                result.Append(token.Data);
            }

            return result.ToString();
        }

        public IEnumerable<IRule> ToRules()
        {
            return Sections.Select(section => section.ToRule());
        }
    }
}
