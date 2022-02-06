using System;
using System.Collections.Generic;
using System.Linq;

using MicaForEveryone.Models;

namespace MicaForEveryone.Config
{
    public class Section
    {
        public static Section Create(SectionType type, string parameter, KeyValuePair<KeyName, object>[] pairs)
        {
            var tokens = new List<Token>();
            tokens.Add(new Token(LexicalTokenType.Identifier, TokenType.SectionType, type.ToString()));
            if (parameter != null)
            {
                tokens.Add(new Token(LexicalTokenType.Operator, TokenType.SectionParameterStart, ":"));
                tokens.Add(new Token(LexicalTokenType.Space, TokenType.Space, " "));
                tokens.Add(new Token(LexicalTokenType.StringLiteral, TokenType.SectionParameter, parameter));
                tokens.Add(new Token(LexicalTokenType.Space, TokenType.Space, " "));
            }
            tokens.Add(new Token(LexicalTokenType.Operator, TokenType.SectionStart, "{"));
            tokens.Add(new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""));
            foreach (var pair in pairs)
            {
                tokens.Add(new Token(LexicalTokenType.Space, TokenType.Space, "    "));
                tokens.Add(new Token(LexicalTokenType.Identifier, TokenType.KeyName, pair.Key.ToString()));
                tokens.Add(new Token(LexicalTokenType.Operator, TokenType.KeySet, "="));
                tokens.Add(new Token(LexicalTokenType.Identifier, TokenType.KeyValue, pair.Value.ToString()));
                tokens.Add(new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""));
            }
            tokens.Add(new Token(LexicalTokenType.Operator, TokenType.SectionEnd, "}"));
            var parser = new Parser(tokens.ToArray());
            var result = parser.ParseSection();
            result.PreTokens = new[]
            {
                new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""),
                new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""),
            };
            return result;
        }

        public Section(EvaluatedSymbol<SectionType> type, Symbol parameter)
        {
            Type = type;
            Parameter = parameter;
            Name = Type.Value switch
            {
                SectionType.Global => "Global",
                SectionType.Process => $"Process({Parameter!.Name})",
                SectionType.Class => $"Class({Parameter!.Name})",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public EvaluatedSymbol<SectionType> Type { get; }

        public Symbol Parameter { get; }

        public string Name { get; }

        public Dictionary<EvaluatedSymbol<KeyName>, Symbol> Pairs { get; } = new();

        internal Token[] Tokens { get; set; }

        internal Token[] PreTokens { get; set; }

        public EvaluatedSymbol<TitlebarColorMode> GetTitleBarColor()
        {
            return (EvaluatedSymbol<TitlebarColorMode>)
                Pairs.First(pair => pair.Key.Value == KeyName.TitleBarColor).Value;
        }

        public EvaluatedSymbol<BackdropType> GetBackdropPreference()
        {
            return (EvaluatedSymbol<BackdropType>)
                Pairs.First(pair => pair.Key.Value == KeyName.BackdropPreference).Value;
        }

        public EvaluatedSymbol<bool> GetExtendFrameIntoClientArea()
        {
            return (EvaluatedSymbol<bool>)
                Pairs.First(pair => pair.Key.Value == KeyName.ExtendFrameToClientArea).Value;
        }
    }
}
