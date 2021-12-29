using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MicaForEveryone.Rules;

namespace MicaForEveryone.Config
{
    public class Document
    {
        public static Document Parse(TextReader source)
        {
            var tokens = new Tokenizer(new Lexer(source).Parse()).Parse();
            return new Parser(tokens).Parse();
        }

        private readonly Token[] _tokens;

        internal Document(Token[] tokens)
        {
            _tokens = tokens;
        }

        public IList<Section> Sections { get; } = new List<Section>();

        public void Save(TextWriter writer)
        {
            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.NewLine)
                {
                    writer.WriteLine();
                }
                else
                {
                    writer.Write(token.Data);
                }
            }
        }

        public IEnumerable<IRule> ToRules()
        {
            return Sections.Select(section => section.ToRule());
        }
    }
}
