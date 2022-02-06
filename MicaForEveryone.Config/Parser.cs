using System;
using System.Collections.Generic;
using System.Linq;

using MicaForEveryone.Models;

namespace MicaForEveryone.Config
{
    internal class Parser
    {
        private int _position;

        public Parser(Token[] data)
        {
            Data = data;
        }

        private Token[] Data { get; }
        private Token CurrentToken => Data[_position];

        public Document ParseDocument()
        {
            var result = new Document();

            int start = _position;
            Section section = null;

            while (_position < Data.Length)
            {
                ExpectToken(TokenType.SectionType);

                var preTokens = new Token[_position - start];
                if (_position > start)
                {
                    Array.Copy(Data, start, preTokens, 0, preTokens.Length);
                }

                section = ParseSection();
                section.PreTokens = preTokens;
                result.Sections.Add(section);

                start = ++_position;
                SkipSpace();
            }

            if (_position > start && section != null)
            {
                section.Tokens = new Token[section.Tokens.Length + _position - start - 1];
                Array.Copy(Data, Data.Length - section.Tokens.Length - 1, section.Tokens, 0, section.Tokens.Length);
            }

            return result;
        }

        public Section ParseSection()
        {
            var start = _position;

            ExpectToken(TokenType.SectionType);
            var type = new EvaluatedSymbol<SectionType>(CurrentToken);

            if (type.Value == SectionType.Global)
            {
                NextToken(TokenType.SectionStart);
            }
            else
            {
                NextToken(TokenType.SectionParameterStart);
            }

            var parameter = CurrentToken.Type switch
            {
                TokenType.SectionParameterStart => GetNextSymbol(TokenType.SectionParameter),
                TokenType.SectionStart => null,
                _ => throw new UnexpectedTokenError(CurrentToken, $"Expected Section Parameter or Section Start, found `{CurrentToken.Data}`"),
            };

            var section = new Section(type, parameter);

            if (CurrentToken.Type == TokenType.SectionParameter)
                NextToken(TokenType.SectionStart);

            while (_position < Data.Length)
            {
                NextToken();
                if (CurrentToken.Type == TokenType.SectionEnd)
                    break;

                ExpectToken(TokenType.KeyName);
                var name = new EvaluatedSymbol<KeyName>(CurrentToken);

                NextToken(TokenType.KeySet);

                NextToken(TokenType.KeyValue);
                Symbol value = name.Value switch
                {
                    KeyName.TitleBarColor => new EvaluatedSymbol<TitlebarColorMode>(CurrentToken),
                    KeyName.BackdropPreference => new EvaluatedSymbol<BackdropType>(CurrentToken),
                    KeyName.ExtendFrameToClientArea => new EvaluatedSymbol<bool>(CurrentToken),
                    _ => throw new ArgumentOutOfRangeException(),
                };

                section.Pairs.Add(name, value);
            }

            section.Tokens = new Token[_position - start + 1];
            Array.Copy(Data, start, section.Tokens, 0, section.Tokens.Length);

            return section;
        }

        private void SkipSpace()
        {
            while (_position < Data.Length)
            {
                if (CurrentToken.Type is not (TokenType.Space or TokenType.Comment or TokenType.NewLine))
                    break;
                _position++;
            }
        }

        private void NextToken()
        {
            _position++;
            SkipSpace();
            if (_position >= Data.Length)
                throw new UnexpectedEndOfFile(Data[Data.Length - 1]);
        }

        private void ExpectToken(TokenType expected)
        {
            SkipSpace();
            if (_position >= Data.Length)
                throw new UnexpectedEndOfFile(Data[Data.Length - 1]);
            if (CurrentToken.Type != expected)
                throw new UnexpectedTokenError(CurrentToken, $"Expected token of type {expected}, found {CurrentToken.Type}");
        }

        private void NextToken(TokenType expected)
        {
            NextToken();
            if (CurrentToken.Type != expected)
                throw new UnexpectedTokenError(CurrentToken, $"Expected token of type {expected}, found {CurrentToken.Type}");
        }

        private Symbol GetNextSymbol(TokenType expected)
        {
            NextToken(expected);
            return new Symbol(CurrentToken);
        }
    }
}
