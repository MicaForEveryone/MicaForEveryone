using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MicaForEveryone.Config
{
    public class Context
    {
        public Context()
        {
            TypeMap = new TypeMap(this);
            Parser = new Parser.Parser(this);
        }

        public TypeMap TypeMap { get; }

        internal Parser.Parser Parser { get; }

        internal Tokenizer.Tokenizer Tokenizer { get; } = new();

        public async Task<XclDocument> ParseDocumentAsync(TextReader input)
        {
            Tokenizer.ResetPosition();
            Parser.ResetPosition();
            var lexer = new Lexer.Lexer(input);
            Tokenizer.Data = await lexer.ParseAsync();
            Parser.Data = Tokenizer.Parse();
            return Parser.ParseDocument();
        }
    }
}
