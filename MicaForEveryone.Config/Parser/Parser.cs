using System;
using System.Collections.Generic;
using System.Linq;

using MicaForEveryone.Config.Tokenizer;

namespace MicaForEveryone.Config.Parser
{
    internal class Parser
    {
        private readonly Context _context;

        private int _position;

        public Parser(Context context)
        {
            _context = context;
        }

        public Token[] Data { get; set; }
        public Token CurrentToken => Data[_position];

        public void ResetPosition()
        {
            _position = 0;
        }

        public XclDocument ParseDocument()
        {
            var result = new XclDocument(_context);

            int start = _position;
            XclInstance section = null;

            while (_position < Data.Length)
            {
                ExpectToken(TokenType.TypeName);

                var preTokens = new Token[_position - start];
                if (_position > start)
                {
                    Array.Copy(Data, start, preTokens, 0, preTokens.Length);
                }

                section = ParseSection(result);
                section.PreTokens = preTokens;
                result.Instances.Add(section);

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

        public XclInstance ParseSection(XclDocument document)
        {
            var start = _position;

            ExpectToken(TokenType.TypeName);
            var type = document.GetXclClass(CurrentToken.Data);

            if (type.ParameterType == null)
            {
                NextToken(TokenType.SectionStart);
            }
            else
            {
                NextToken(TokenType.ParameterStart);
                NextToken(TokenType.Value);
            }

            var parameter = CurrentToken.Type == TokenType.Value && type.ParameterType != null ? 
                type.ParameterType.SymbolToValue(new Symbol(CurrentToken)) :
                null;

            var section = new XclInstance(null, type, parameter);

            if (CurrentToken.Type == TokenType.Value)
                NextToken(TokenType.SectionStart);

            while (_position < Data.Length)
            {
                NextToken();
                if (CurrentToken.Type == TokenType.SectionEnd)
                    break;

                ExpectToken(TokenType.FieldName);
                var field = type.GetField(CurrentToken.Data);

                if (field == null)
                {
                    throw new ParserError(CurrentToken, "Field not found.");
                }

                NextToken(TokenType.SetOperator);

                NextToken(TokenType.Value);
                var value = field.Type.SymbolToValue(new Symbol(CurrentToken));

                section.SetValue(field, value);
            }

            section.Tokens = new Token[_position - start + 1];
            Array.Copy(Data, start, section.Tokens, 0, section.Tokens.Length);

            return section;
        }

        private void SkipSpace()
        {
            while (_position < Data.Length)
            {
                if (CurrentToken.Type != TokenType.Meaningless)
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
    }
}
