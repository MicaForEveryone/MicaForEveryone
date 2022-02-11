using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MicaForEveryone.Config.Reflection;

namespace MicaForEveryone.Config
{
    public class XclDocument
    {
        internal XclDocument(Context context)
        {
            Types = new Dictionary<string, XclType>(context.TypeMap.XclTypes);
        }

        internal Dictionary<string, XclType> Types { get; }

        public IList<XclInstance> Instances { get; } = new List<XclInstance>();

        public XclType GetXclType(string typeName)
        {
            return Types[typeName];
        }

        public XclClass GetXclClass(string className)
        {
            return Types[className] as XclClass;
        }

        public async Task SaveAsync(TextWriter writer)
        {
            // write each section
            foreach (var section in Instances)
            {
                if (section.PreTokens != null)
                    await WriteTokensAsync(writer, section.PreTokens);
                await WriteTokensAsync(writer, section.GenerateTokens());
            }
        }

        private async Task WriteTokensAsync(TextWriter writer, IEnumerable<Tokenizer.Token> tokens)
        {
            // write each token
            foreach (var token in tokens)
            {
                if (token.LexerType == Lexer.TokenType.NewLine)
                {
                    // write line if it's a new line token
                    await writer.WriteLineAsync();
                }
                else if (token.LexerType == Lexer.TokenType.StringLiteral)
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
