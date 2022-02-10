using System.Diagnostics;

namespace MicaForEveryone.Config.Tokenizer
{
    [DebuggerDisplay("{Type}: {Data} ({Line}:{Column})")]
    internal class Token : IToken
    {
        private readonly Lexer.Token _lexerToken;

        internal Token(TokenType type, Lexer.Token lexerToken)
        {
            Type = type;
            _lexerToken = lexerToken;
        }

        internal Token(Lexer.TokenType lexerType, TokenType type, string data) : 
            this(type, new Lexer.Token(lexerType, data, -1, -1))
        {
        }

        public TokenType Type { get; }

        public Lexer.TokenType LexerType => _lexerToken.Type;
        public string Data
        {
            get => _lexerToken?.Data;
            set => _lexerToken.Data = value;
        }
        public int Line => _lexerToken.Line;
        public int Column => _lexerToken.Column;
    }
}
