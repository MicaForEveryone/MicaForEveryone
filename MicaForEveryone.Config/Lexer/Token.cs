using System.Diagnostics;

namespace MicaForEveryone.Config.Lexer
{
    [DebuggerDisplay("{Type}: {Data} ({Line}:{Column})")]
    internal class Token : IToken
    {
        internal Token(TokenType type, string data, int line, int column)
        {
            Type = type;
            Data = data;
            Line = line;
            Column = column;
        }

        public TokenType Type { get; }
        public string Data { get; set; }
        public int Line { get; }
        public int Column { get; }
    }
}
