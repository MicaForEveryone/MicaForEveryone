using System;
using System.Diagnostics;

namespace MicaForEveryone.Config.Parser
{
    /// <summary>
    /// Symbol represents an identifier in config file
    /// </summary>
    [DebuggerDisplay("{Name} ({Line}:{Column})")]
    public class Symbol : IToken
    {
        internal Symbol(Tokenizer.Token token)
        {
            Token = token;
        }

        internal Symbol(Lexer.TokenType lexerType, Tokenizer.TokenType tokenizerType, string data)
        {
            Token = new Tokenizer.Token(lexerType, tokenizerType, data);
        }

        /// <summary>
        /// name of symbol as string
        /// </summary>
        public string Name
        {
            get => Token?.Data;
            set => Token.Data = value;
        }

        public int Line => Token?.Line ?? -1;

        public int Column => Token?.Column ?? -1;

        string IToken.Data => Token?.Data;

        internal Tokenizer.Token Token { get; }
    }
}
