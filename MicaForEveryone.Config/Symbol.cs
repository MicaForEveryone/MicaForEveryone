using System;
using System.Diagnostics;

namespace MicaForEveryone.Config
{
    public enum SectionType
    {
        Global,
        Process,
        Class,
    }

    public enum KeyName
    {
        TitleBarColor,
        BackdropPreference,
        ExtendFrameToClientArea,
    }

    [DebuggerDisplay("{Name} ({Line}:{Column})")]
    public class Symbol
    {
        internal Symbol(Token token)
        {
            Token = token;
        }

        public string Name
        {
            get => Token.Data;
            set => Token.Data = value;
        }

        public int Line => Token.Line;

        public int Column => Token.Column;

        internal Token Token { get; }
    }

    public class EvaluatedSymbol<TValue> : Symbol
    {
        private TValue _value;

        internal EvaluatedSymbol(Token token) : base(token)
        {
            var type = typeof(TValue);
            if (type.IsEnum)
            {
                try
                {
                    _value = (TValue)Enum.Parse(typeof(TValue), Name);
                }
                catch (ArgumentException)
                {
                    var values = string.Join("\n", typeof(TValue).GetEnumNames());
                    throw new InvalidSymbolError(this, "Invalid Symbol, Possible Values are: \n" + values);
                }
            }
            else if (type == typeof(bool))
            {
                try
                {
                    _value = (TValue)(object)bool.Parse(Name);
                }
                catch (FormatException)
                {
                    throw new InvalidSymbolError(this, "Invalid Boolean Value, Should be `True` or `False`.");
                }
            }
            else if (type == typeof(string))
            {
                _value = (TValue)(object)Name;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal EvaluatedSymbol(Symbol parent, TValue value) : base(parent.Token)
        {
            _value = value;
        }

        public TValue Value
        {
            get => _value;
            set
            {
                _value = value;
                Token.Data = value.ToString();
            }
        }
    }
}
