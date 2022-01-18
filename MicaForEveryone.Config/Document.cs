using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MicaForEveryone.Config
{
    public class Document
    {
        public static async Task<Document> ParseAsync(TextReader source)
        {
            var lexTokens = await new Lexer(source).ParseAsync();
            var tokens = new Tokenizer(lexTokens).Parse();
            return new Parser(tokens).Parse();
        }

        public static Document Empty => new(new Token[0]);

        private Token[] _tokens;

        internal Document(Token[] tokens)
        {
            _tokens = tokens;
        }

        public IList<Section> Sections { get; } = new List<Section>();

        public async Task SaveAsync(TextWriter writer)
        {
            foreach (var token in _tokens)
            {
                if (token.Type == TokenType.NewLine)
                {
                    await writer.WriteLineAsync();
                }
                else
                {
                    await writer.WriteAsync(token.Data);
                }
            }
        }

        public void AddNewSection(SectionType type, string parameter, KeyValuePair<KeyName, object>[] pairs)
        {
            var tokens = _tokens.ToList();
            tokens.Add(new Token(TokenType.SectionType, type.ToString()));
            if (parameter != null)
            {
                tokens.Add(new Token(TokenType.SectionParameterStart, ":"));
                tokens.Add(new Token(TokenType.SectionParameter, parameter));
            }
            tokens.Add(new Token(TokenType.SectionStart, "{"));
            tokens.Add(new Token(TokenType.NewLine, ""));
            foreach (var pair in pairs)
            {
                tokens.Add(new Token(TokenType.KeyName, pair.Key.ToString()));
                tokens.Add(new Token(TokenType.KeySet, "="));
                tokens.Add(new Token(TokenType.KeyValue, pair.Value.ToString()));
                tokens.Add(new Token(TokenType.NewLine, ""));
            }
            tokens.Add(new Token(TokenType.SectionEnd, "}"));
            _tokens = tokens.ToArray();
            var parser = new Parser(_tokens, this);
            parser.Parse();
        }
    }
}
