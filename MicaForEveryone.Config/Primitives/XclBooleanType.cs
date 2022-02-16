using System;
using System.Collections.Generic;
using System.Text;

using MicaForEveryone.Config.Parser;
using MicaForEveryone.Config.Reflection;

#nullable enable

namespace MicaForEveryone.Config.Primitives
{
    public class XclBooleanType : XclType
    {
        public static XclBooleanType Instance { get; } = new();

        internal XclBooleanType() : base("bool")
        {
        }

        internal override XclValue SymbolToValue(Symbol symbol)
        {
            try
            {
                return new XclValue(symbol.Token, this, bool.Parse(symbol.Name));
            }
            catch (FormatException)
            {
                throw new ParserError(symbol, $"Can't parse string `{symbol.Name}` to type boolean.");
            }
        }

        internal override Symbol ValueToSymbol(XclValue value)
        {
            return new Symbol(Lexer.TokenType.Identifier, Tokenizer.TokenType.Value, GetTokenData(value));
        }
    }
}
