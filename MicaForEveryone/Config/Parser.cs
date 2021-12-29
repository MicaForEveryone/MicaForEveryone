using System;

using MicaForEveryone.Models;

namespace MicaForEveryone.Config
{
    internal class Parser
    {
        private readonly Document _result;
        private int _position;

        public Parser(Token[] data)
        {
            Data = data;
            _result = new Document(data);
        }

        private Token[] Data { get; }
        private Token CurrentToken => Data[_position];

        private void SkipSpace()
        {
            while (_position < Data.Length)
            {
                if (CurrentToken.Type is not (TokenType.Space or TokenType.Comment))
                    break;
                _position++;
            }
        }

        private void NextToken()
        {
            _position++;
            SkipSpace();
            if (_position >= Data.Length)
                throw new UnexpectedEndOfFile(Data[^1]);
        }

        private void ExpectToken(TokenType expected)
        {
            SkipSpace();
            if (_position >= Data.Length)
                throw new UnexpectedEndOfFile(Data[^1]);
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

        public Document Parse()
        {
            if (_result.Sections.Count > 0)
                return _result;

            while (_position < Data.Length)
            {
                ExpectToken(TokenType.SectionType);
                ParseSection();
                _position++;
                SkipSpace();
            }

            return _result;
        }

        private void ParseSection()
        {
            ExpectToken(TokenType.SectionType);
            var type = new EvaluatedSymbol<SectionType>(CurrentToken);

            NextToken();
            var parameter = CurrentToken.Type switch
            {
                TokenType.SectionParameterStart => GetNextSymbol(TokenType.SectionParameter),
                TokenType.SectionStart => null,
                _ => throw new UnexpectedTokenError(CurrentToken, $"Expected Section Parameter or Section Start, found `{CurrentToken.Data}`"),
            };

            var rule = new Section(type, parameter);

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

                rule.Pairs.Add(name, value);
            }

            _result.Sections.Add(rule);
        }

    }
}
