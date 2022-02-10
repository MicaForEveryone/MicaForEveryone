using System;
using System.Collections.Generic;
using System.Text;

using MicaForEveryone.Config.Parser;
using MicaForEveryone.Config.Reflection;

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
            return new XclValue(symbol.Token, this, bool.Parse(symbol.Name));
        }

        internal override Symbol ValueToSymbol(XclValue value)
        {
            return new Symbol(Lexer.TokenType.Identifier, Tokenizer.TokenType.Value, GetTokenData(value));
        }
    }
}
