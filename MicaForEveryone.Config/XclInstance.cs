using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MicaForEveryone.Config.Reflection;
using MicaForEveryone.Config.Tokenizer;

namespace MicaForEveryone.Config
{
    [DebuggerDisplay("{Type} {Parameter}")]
    public class XclInstance : XclValue
    {
        private Dictionary<XclField, XclValue> _pairs = new();

        internal XclInstance(Token token, XclClass type, XclValue parameter) :
            this(token, type, parameter, type.CreateInstance(parameter))
        {
        }

        internal XclInstance(Token token, XclClass type, XclValue parameter, object value) : base(token, type, value)
        {
            Parameter = parameter;

            foreach (var field in type.GetFields())
            {
                _pairs.Add(field, field.Type.ToXclValue(type.GetFieldValue(this, field)));
            }
        }

        public XclValue Parameter { get; }

        // save to keep comments and spaces while saving changes
        internal Token[] PreTokens { get; set; }
        internal Token[] Tokens { get; set; }

        public XclValue GetValue(XclField field)
        {
            return _pairs[field];
        }

        public void SetValue(XclField field, XclValue value)
        {
            if (Type is XclClass xclClass)
                xclClass.SetFieldValue(this, field, value);
            _pairs[field] = value;
        }

        /// <summary>
        /// Call after you changed fields of <see cref="Value"/> directly.
        /// </summary>
        public void UpdateData()
        {
            var type = (XclClass)Type;
            foreach (var pair in _pairs)
            {
                pair.Value.Value = type.GetFieldValue(this, pair.Key);
            }
        }

        internal IEnumerable<Token> GenerateTokens()
        {
            if (Tokens != null)
                return Tokens;

            // result container
            var tokens = new List<Token>
            {
                new Token(Lexer.TokenType.NewLine, TokenType.Meaningless, ""),
                new Token(Lexer.TokenType.NewLine, TokenType.Meaningless, ""),
            };

            // template: `type: "parameter" {`
            // a section starts with type
            tokens.Add(new Token(Lexer.TokenType.Identifier, TokenType.TypeName, Type.Name));

            // add parameter if there's one
            if (Parameter != null)
            {
                tokens.Add(new Token(Lexer.TokenType.Operator, TokenType.ParameterStart, ":"));
                tokens.Add(new Token(Lexer.TokenType.Space, TokenType.Meaningless, " "));
                tokens.Add(Parameter.Type.ValueToSymbol(Parameter).Token);
            }

            tokens.Add(new Token(Lexer.TokenType.Space, TokenType.Meaningless, " "));
            tokens.Add(new Token(Lexer.TokenType.Operator, TokenType.SectionStart, "{"));
            tokens.Add(new Token(Lexer.TokenType.NewLine, TokenType.Meaningless, ""));

            // template: `    Key=Value`
            foreach (var pair in _pairs)
            {
                tokens.Add(new Token(Lexer.TokenType.Space, TokenType.Meaningless, "  "));

                tokens.Add(pair.Key.Token);
                tokens.Add(new Token(Lexer.TokenType.Space, TokenType.Meaningless, " "));

                tokens.Add(new Token(Lexer.TokenType.Operator, TokenType.SetOperator, "="));
                tokens.Add(new Token(Lexer.TokenType.Space, TokenType.Meaningless, " "));

                tokens.Add(pair.Value.Type.ValueToSymbol(pair.Value).Token);

                tokens.Add(new Token(Lexer.TokenType.NewLine, TokenType.Meaningless, ""));
            }

            // add section end
            tokens.Add(new Token(Lexer.TokenType.Operator, TokenType.SectionEnd, "}"));

            return tokens;
        }
    }
}
