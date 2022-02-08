using System;
using System.Diagnostics;

namespace MicaForEveryone.Config
{
    /// <summary>
    /// possible types of section
    /// </summary>
    public enum SectionType
    {
        Global,
        Process,
        Class,
    }

    /// <summary>
    /// possible key names
    /// </summary>
    public enum KeyName
    {
        TitleBarColor,
        BackdropPreference,
        ExtendFrameToClientArea,
    }

    /// <summary>
    /// Symbol represents an identifier in config file
    /// </summary>
    [DebuggerDisplay("{Name} ({Line}:{Column})")]
    public class Symbol
    {
        internal Symbol(Token token)
        {
            Token = token;
        }

        /// <summary>
        /// name of symbol as string
        /// </summary>
        public string Name
        {
            get => Token.Data;
            set => Token.Data = value;
        }

        public int Line => Token.Line;

        public int Column => Token.Column;

        internal Token Token { get; }
    }

    /// <summary>
    /// EvaluatedSymbol represents <see cref="Symbol"/> that its value is evaluated from <see cref="string"/> to <see cref="TValue"/>.
    /// </summary>
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

        /// <summary>
        /// evaluated value of symbol
        /// </summary>
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
