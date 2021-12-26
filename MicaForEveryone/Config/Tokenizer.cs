using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MicaForEveryone.Config
{
    public enum TokenType
    {
        NewSection,
        SectionData,
        EndSection,
        KeyName,
        KeyValue,
    }

    [DebuggerDisplay("{Type}: {Data} ({Line}:{Column})")]
    public class Token
    {
        public Token(TokenType type, string data, int line, int column)
        {
            Type = type;
            Data = data;
            Line = line;
            Column = column;
        }

        public TokenType Type { get; }
        public string Data { get; }
        public int Line { get; }
        public int Column { get; }
    }

    public class Tokenizer
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

                    case LexicalTokenType.Operator:
                        ThrowException();
                        break;

                    case LexicalTokenType.Space:
                        break;

                    case LexicalTokenType.Comment:
                        break;
                }
                _position++;
            }

            return _tokens.ToArray();
        }

        private void SkipSpace()
        {
            while (_position < Data.Length)
            {
                if (CurrentToken.Type is not (LexicalTokenType.Space or LexicalTokenType.Comment))
                    break;
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
            AddToken(TokenType.NewSection);
            _position++;
            SkipSpace();

            if (CurrentToken.Type != LexicalTokenType.Operator)
                ThrowException();

            if (CurrentToken.Data == ":")
            {
                _position++;

                SkipSpace();

                if (CurrentToken.Type is not (LexicalTokenType.Identifier or LexicalTokenType.StringLiteral))
                    ThrowException();
                AddToken(TokenType.SectionData);
                _position++;

                SkipSpace();

                if (CurrentToken.Type != LexicalTokenType.Operator && CurrentToken.Data != "{")
                    ThrowException();
                _position++;
            }
            else if (CurrentToken.Data == "{")
            {
                _tokens.Add(new Token(TokenType.SectionData, "", CurrentToken.Line, CurrentToken.Column));
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
                    AddToken(TokenType.EndSection);
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
            _position++;

            SkipSpace();

            if (CurrentToken.Type is not (LexicalTokenType.Identifier or LexicalTokenType.StringLiteral))
                ThrowException();
            AddToken(TokenType.KeyValue);
            _position++;
        }
    }
}
