using System;

using MicaForEveryone.Config.Parser;

namespace MicaForEveryone.Config.Reflection
{
    public class XclType : Symbol
    {
        internal XclType(string name) : 
            base(new Tokenizer.Token(Lexer.TokenType.Identifier, Tokenizer.TokenType.TypeName, name))
        {
        }

        public Context Context { get; internal set; }

        public virtual XclValue ToXclValue(object value)
        {
            return new XclValue(this, value);
        }

        internal virtual XclValue SymbolToValue(Symbol symbol)
        {
            return new XclValue(symbol.Token, this, symbol.Name);
        }

        internal virtual Symbol ValueToSymbol(XclValue value)
        {
            return new Symbol(Lexer.TokenType.StringLiteral, Tokenizer.TokenType.Value, value.Value.ToString());
        }

        internal virtual string GetTokenData(XclValue value)
        {
            return value.Value.ToString();
        }
    }
}
