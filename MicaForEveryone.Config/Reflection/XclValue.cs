using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MicaForEveryone.Config.Parser;

namespace MicaForEveryone.Config.Reflection
{
    [DebuggerDisplay("{Type} {Value}")]
    public class XclValue : Symbol
    {
        private object _value;

        public XclValue(XclType type, object value) :
this(null, type, value)
        {
        }

        internal XclValue(Tokenizer.Token token, XclType type, object value) : base(token)
        {
            Type = type;
            Value = value;
        }

        public XclType Type { get; }

        public object Value
        {
            get => _value;
            set
            {
                _value = value;

                if (Token != null)
                    Token.Data = Type.GetTokenData(this);
            }
        }
    }
}