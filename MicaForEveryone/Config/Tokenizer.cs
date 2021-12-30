using System;
using System.Collections.Generic;

namespace MicaForEveryone.Config
{
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
                        throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Section Name, found `{CurrentToken.Data}`");

                    case LexicalTokenType.NewLine:
                        AddToken(TokenType.NewLine);
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
                else if (CurrentToken.Type == LexicalTokenType.NewLine)
                {
                    AddToken(TokenType.NewLine);
                }
                else
                {
                    break;
                }
                _position++;
            }
        }

        private void AddToken(TokenType type)
        {
            _tokens.Add(new Token(type, CurrentToken.Data, CurrentToken.Line, CurrentToken.Column));
        }

        private void ParseSection()
        {
            if (CurrentToken.Type != LexicalTokenType.Identifier)
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Section Name, found `{CurrentToken.Data}`");
            AddToken(TokenType.SectionType);
            _position++;
            SkipSpace();

            if (CurrentToken.Type == LexicalTokenType.Operator && CurrentToken.Data == ":")
            {
                AddToken(TokenType.SectionParameterStart);
                _position++;

                SkipSpace();

                if (CurrentToken.Type is not (LexicalTokenType.Identifier or LexicalTokenType.StringLiteral))
                    throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Section Parameter, found `{CurrentToken.Data}`");
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
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected `:` or `{{`, found `{CurrentToken.Data}`");
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
                    throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Key/Value Pair or `}}`, found `{CurrentToken.Data}`");
                }
            }
        }

        private void ParsePair()
        {
            if (CurrentToken.Type != LexicalTokenType.Identifier)
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Key Name, found `{CurrentToken.Data}`");
            AddToken(TokenType.KeyName);
            _position++;

            SkipSpace();

            if (CurrentToken.Type != LexicalTokenType.Operator || CurrentToken.Data != "=")
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected `=`, found `{CurrentToken.Data}`");
            AddToken(TokenType.KeySet);
            _position++;

            SkipSpace();

            if (CurrentToken.Type is not (LexicalTokenType.Identifier or LexicalTokenType.StringLiteral))
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Value, found `{CurrentToken.Data}`");
            AddToken(TokenType.KeyValue);
            _position++;
        }
    }
}
