using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MicaForEveryone.Config
{
    /// <summary>
    /// Instance of Config Document
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Read and Parse config file from <paramref name="source"/>
        /// </summary>
        public static async Task<Document> ParseAsync(TextReader source)
        {
            var lexTokens = await new Lexer(source).ParseAsync();
            var tokens = new Tokenizer(lexTokens).Parse();
            return new Parser(tokens).ParseDocument();
        }

        /// <summary>
        /// Get an empty document
        /// </summary>
        public static Document Empty => new();

        internal Document()
        {
        }

        /// <summary>
        /// Sections of Config Document
        /// </summary>
        public IList<Section> Sections { get; } = new List<Section>();

        /// <summary>
        /// Save config file to <paramref name="writer"/>
        /// </summary>
        public async Task SaveAsync(TextWriter writer)
        {
            // write each section
            foreach (var section in Sections)
            {
                await WriteTokensAsync(writer, section.PreTokens);
                await WriteTokensAsync(writer, section.Tokens);
            }
        }

        private async Task WriteTokensAsync(TextWriter writer, IEnumerable<Token> tokens)
        {
            // write each token
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.NewLine)
                {
                    // write line if it's a new line token
                    await writer.WriteLineAsync();
                }
                else if (token.LexialType == LexicalTokenType.StringLiteral)
                {
                    // write in qoute if it's a string literal
                    await writer.WriteAsync($"\"{token.Data}\"");
                }
                else
                {
                    await writer.WriteAsync(token.Data);
                }
            }
        }
    }
}
