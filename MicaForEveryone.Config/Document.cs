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
            return new Parser(tokens).ParseDocument();
        }

        public static Document Empty => new();

        internal Document()
        {
        }

        public IList<Section> Sections { get; } = new List<Section>();

        public async Task SaveAsync(TextWriter writer)
        {
            foreach (var section in Sections)
            {
                await WriteTokensAsync(writer, section.PreTokens);
                await WriteTokensAsync(writer, section.Tokens);
            }
        }

        private async Task WriteTokensAsync(TextWriter writer, IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.NewLine)
                {
                    await writer.WriteLineAsync();
                }
                else
                {
                    if (token.LexialType == LexicalTokenType.StringLiteral)
                        await writer.WriteAsync($"\"{token.Data}\"");
                    else
                        await writer.WriteAsync(token.Data);
                }
            }
        }
    }
}
