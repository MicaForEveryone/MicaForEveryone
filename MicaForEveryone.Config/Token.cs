using System.Diagnostics;

namespace MicaForEveryone.Config
{
    public interface IToken
    {
        int Line { get; }
        int Column { get; }
        string Data { get; }
    }

    internal enum TokenType
    {
        SectionType,
        SectionParameterStart,
        SectionParameter,
        SectionStart,
        SectionEnd,
        KeyName,
        KeySet,
        KeyValue,
        Space,
        Comment,
        NewLine,
    }

    internal enum LexicalTokenType
    {
        Identifier,
        Operator,
        Space,
        StringLiteral,
        Comment,
        NewLine,
    }

    [DebuggerDisplay("{Type}: {Data} ({Line}:{Column})")]
    internal class LexicalToken : IToken
    {
        public LexicalToken(LexicalTokenType type, string data, int line, int column)
        {
            Type = type;
            Data = data;
            Line = line;
            Column = column;
        }

        public LexicalTokenType Type { get; }
        public string Data { get; }
        public int Line { get; }
        public int Column { get; }
    }

    [DebuggerDisplay("{Type}: {Data} ({Line}:{Column})")]
    internal class Token : IToken
    {
        public Token(TokenType type, string data) : this(type, data, -1, -1)
        {

        }

        public Token(TokenType type, string data, int line, int column)
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
