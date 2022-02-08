using System.Diagnostics;

namespace MicaForEveryone.Config
{
    /// <summary>
    /// Token is a string with a position from config file
    /// </summary>
    public interface IToken
    {
        int Line { get; }
        int Column { get; }
        string Data { get; }
    }

    /// <summary>
    /// Types of Tokenizer tokens
    /// </summary>
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

    /// <summary>
    /// Types of Lexer tokens
    /// </summary>
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
        public Token(LexicalTokenType lexicalType, TokenType type, string data) : this(lexicalType, type, data, -1, -1)
        {

        }

        public Token(LexicalTokenType lexicalType, TokenType type, string data, int line, int column)
        {
            LexialType = lexicalType;
            Type = type;
            Data = data;
            Line = line;
            Column = column;
        }

        public LexicalTokenType LexialType { get; }

        public TokenType Type { get; }
        public string Data { get; set; }
        public int Line { get; }
        public int Column { get; }
    }
}
