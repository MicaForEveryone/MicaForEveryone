using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MicaForEveryone.Rules;

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
        public Symbol(string name)
        {
            Name = name;
        }

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

        public IRule MatchRule(IEnumerable<IRule> rules)
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new Exception($"rule match expected at {Line}:{Column}");
            var result = rules.FirstOrDefault(rule => rule.Name == Name);
            if (result == null)
                throw new Exception($"invalid rule match '{Name}' at {Line}:{Column}");
            return result;
        }
    }

    public class EvaluatedSymbol<TValue> : Symbol
    {
        private TValue _value;

        internal EvaluatedSymbol(Token token) : base(token)
        {
            var type = typeof(TValue);
            if (type.IsEnum)
            {
                _value = (TValue)Enum.Parse(typeof(TValue), Name);
            }
            else if (type == typeof(bool))
            {
                _value = (TValue)(object)bool.Parse(Name);
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
