using System;
using System.Collections.Generic;

namespace MicaForEveryone.Config.Tokenizer
{
    internal class Tokenizer
    {
        private readonly List<Token> _tokens = new();

        private int _position;

        public Tokenizer(Lexer.Token[] data)
        {
            Data = data;
        }

        public Lexer.Token[] Data { get; }
        public Lexer.Token CurrentToken => Data[_position];

        public Token[] Parse()
        {
            if (_tokens.Count > 0)
                return _tokens.ToArray();

            while (_position < Data.Length)
            {
                switch (CurrentToken.Type)
                {
                    case Lexer.TokenType.Identifier:
                        ParseSection();
                        break;

                    case Lexer.TokenType.StringLiteral:
                    case Lexer.TokenType.Operator:
                        throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Section Name, found `{CurrentToken.Data}`");

                    case Lexer.TokenType.NewLine:
                    case Lexer.TokenType.Space:
                    case Lexer.TokenType.Comment:
                        AddToken(TokenType.Meaningless);
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
                if (CurrentToken.Type is Lexer.TokenType.Space
                    or Lexer.TokenType.Comment
                    or Lexer.TokenType.NewLine)
                {
                    AddToken(TokenType.Meaningless);
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
            _tokens.Add(new Token(type, CurrentToken));
        }

        private void ParseSection()
        {
            if (CurrentToken.Type != Lexer.TokenType.Identifier)
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Section Name, found `{CurrentToken.Data}`");
            AddToken(TokenType.TypeName);
            _position++;
            SkipSpace();

            if (CurrentToken.Type == Lexer.TokenType.Operator && CurrentToken.Data == ":")
            {
                AddToken(TokenType.ParameterStart);
                _position++;

                SkipSpace();

                if (CurrentToken.Type is not (Lexer.TokenType.Identifier or Lexer.TokenType.StringLiteral))
                    throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Section Parameter, found `{CurrentToken.Data}`");
                AddToken(TokenType.Value);
                _position++;

                SkipSpace();
            }
            
            if (CurrentToken.Type == Lexer.TokenType.Operator && CurrentToken.Data == "{")
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
                if (CurrentToken.Type == Lexer.TokenType.Identifier)
                {
                    ParsePair();
                }
                else if (CurrentToken.Type == Lexer.TokenType.Operator && CurrentToken.Data == "}")
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
            if (CurrentToken.Type != Lexer.TokenType.Identifier)
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Key Name, found `{CurrentToken.Data}`");
            AddToken(TokenType.FieldName);
            _position++;

            SkipSpace();

            if (CurrentToken.Type != Lexer.TokenType.Operator || CurrentToken.Data != "=")
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected `=`, found `{CurrentToken.Data}`");
            AddToken(TokenType.SetOperator);
            _position++;

            SkipSpace();

            if (CurrentToken.Type is not (Lexer.TokenType.Identifier or Lexer.TokenType.StringLiteral))
                throw new UnexpectedTokenError(CurrentToken, $"Syntax Error:\nExpected Value, found `{CurrentToken.Data}`");
            AddToken(TokenType.Value);
            _position++;
        }
    }
}
