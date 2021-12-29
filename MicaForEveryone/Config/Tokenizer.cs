using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MicaForEveryone.Config
{
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
    }

    [DebuggerDisplay("{Type}: {Data} ({Line}:{Column})")]
    internal class Token
    {
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

    internal class Tokenizer
    {
        private readonly List<Token> _tokens = new();

        private int _position;

        public Tokenizer(LexicalToken[] data)
        {
            Data = data;
        }

        private LexicalToken[] Data { get; }
        private LexicalToken CurrentToken => Data[_position];

        public Token[] Parse()
        {
            if (_tokens.Count > 0)
                return _tokens.ToArray();

            while (_position < Data.Length)
            {
                switch (CurrentToken.Type)
                {
                    case LexicalTokenType.Identifier:
                        ParseSection();
                        break;

                    case LexicalTokenType.StringLiteral:
                    case LexicalTokenType.Operator:
                        ThrowException();
                        break;

                    case LexicalTokenType.Space:
                        AddToken(TokenType.Space);
                        break;

                    case LexicalTokenType.Comment:
                        AddToken(TokenType.Comment);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _position++;
            }

            return _tokens.ToArray();
        }

        private void SkipSpace()
        {
            while (_position < Data.Length)
            {
                if (CurrentToken.Type == LexicalTokenType.Space)
                {
                    AddToken(TokenType.Space);
                }
                else if (CurrentToken.Type == LexicalTokenType.Comment)
                {
                    AddToken(TokenType.Comment);
                }
                else
                {
                    break;
                }
                _position++;
            }
        }

        private void ThrowException()
        {
            throw new FormatException($"unexpected token '{CurrentToken.Data}' at ({CurrentToken.Line}:{CurrentToken.Column})");
        }

        private void AddToken(TokenType type)
        {
            _tokens.Add(new Token(type, CurrentToken.Data, CurrentToken.Line, CurrentToken.Column));
        }

        private void ParseSection()
        {
            if (CurrentToken.Type != LexicalTokenType.Identifier)
                ThrowException();
            AddToken(TokenType.SectionType);
            _position++;
            SkipSpace();

            if (CurrentToken.Type == LexicalTokenType.Operator && CurrentToken.Data == ":")
            {
                AddToken(TokenType.SectionParameterStart);
                _position++;

                SkipSpace();

                if (CurrentToken.Type is not (LexicalTokenType.Identifier or LexicalTokenType.StringLiteral))
                    ThrowException();
                AddToken(TokenType.SectionParameter);
                _position++;

                SkipSpace();
            }
            
            if (CurrentToken.Type == LexicalTokenType.Operator && CurrentToken.Data == "{")
            {
                AddToken(TokenType.SectionStart);
                _position++;
            }
            else
            {
                ThrowException();
            }

            while (_position < Data.Length)
            {
                SkipSpace();
                if (CurrentToken.Type == LexicalTokenType.Identifier)
                {
                    ParsePair();
                }
                else if (CurrentToken.Type == LexicalTokenType.Operator && CurrentToken.Data == "}")
                {
                    AddToken(TokenType.SectionEnd);
                    break;
                }
                else
                {
                    ThrowException();
                }
            }
        }

        private void ParsePair()
        {
            if (CurrentToken.Type != LexicalTokenType.Identifier)
                ThrowException();
            AddToken(TokenType.KeyName);
            _position++;

            SkipSpace();

            if (CurrentToken.Type != LexicalTokenType.Operator || CurrentToken.Data != "=")
                ThrowException();
            AddToken(TokenType.KeySet);
            _position++;

            SkipSpace();

            if (CurrentToken.Type is not (LexicalTokenType.Identifier or LexicalTokenType.StringLiteral))
                ThrowException();
            AddToken(TokenType.KeyValue);
            _position++;
        }
    }
}
