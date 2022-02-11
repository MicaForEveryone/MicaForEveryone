using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MicaForEveryone.Config.Parser;
using MicaForEveryone.Config.Reflection;

namespace MicaForEveryone.Config.Primitives
{
    public class XclEnumType : XclType
    {
        private Type _enumType;

        public XclEnumType(Type enumType) : 
            this((enumType.GetCustomAttributes(typeof(XclTypeAttribute), false).FirstOrDefault() as XclTypeAttribute)?.TypeName, enumType)
        {
        }

        internal XclEnumType(string name, Type enumType) : 
            base(name ?? enumType.Name)
        {
            _enumType = enumType;
        }

        internal override XclValue SymbolToValue(Symbol symbol)
        {
            return new XclValue(symbol.Token, this, Enum.Parse(_enumType, symbol.Name));
        }

        internal override Symbol ValueToSymbol(XclValue value)
        {
            return new Symbol(Lexer.TokenType.Identifier, Tokenizer.TokenType.Value, GetTokenData(value));
        }
    }
}
