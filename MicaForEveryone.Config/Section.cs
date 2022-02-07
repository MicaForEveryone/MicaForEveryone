using System;
using System.Collections.Generic;
using System.Linq;

using MicaForEveryone.Models;

namespace MicaForEveryone.Config
{
    /// <summary>
    /// Config Section, contains a dictionary of properties and their value, under a name and type
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Generate Section with given data
        /// </summary>
        /// <param name="type">type of section</param>
        /// <param name="parameter">name of section, can be null</param>
        /// <param name="pairs">key/value pairs</param>
        /// <returns></returns>
        public static Section Create(SectionType type, string parameter, KeyValuePair<KeyName, object>[] pairs)
        {
            var tokens = new List<Token>(); // result container

            // template: `type: "parameter" {`
            // a section starts with type
            tokens.Add(new Token(LexicalTokenType.Identifier, TokenType.SectionType, type.ToString()));

            // add parameter if there's one
            if (parameter != null)
            {
                tokens.Add(new Token(LexicalTokenType.Operator, TokenType.SectionParameterStart, ":"));
                tokens.Add(new Token(LexicalTokenType.Space, TokenType.Space, " "));
                // section parameter should be literal, it may contain '#' or other charaters that messes with parser
                tokens.Add(new Token(LexicalTokenType.StringLiteral, TokenType.SectionParameter, parameter));
                tokens.Add(new Token(LexicalTokenType.Space, TokenType.Space, " "));
            }

            tokens.Add(new Token(LexicalTokenType.Operator, TokenType.SectionStart, "{"));
            tokens.Add(new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""));

            // template: `    Key=Value`
            foreach (var pair in pairs)
            {
                tokens.Add(new Token(LexicalTokenType.Space, TokenType.Space, "    "));
                tokens.Add(new Token(LexicalTokenType.Identifier, TokenType.KeyName, pair.Key.ToString()));
                tokens.Add(new Token(LexicalTokenType.Operator, TokenType.KeySet, "="));
                tokens.Add(new Token(LexicalTokenType.Identifier, TokenType.KeyValue, pair.Value.ToString()));
                tokens.Add(new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""));
            }

            // add section end
            tokens.Add(new Token(LexicalTokenType.Operator, TokenType.SectionEnd, "}"));

            // parse generated code
            var parser = new Parser(tokens.ToArray());
            var result = parser.ParseSection();
            result.PreTokens = new[]
            {
                new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""),
                new Token(LexicalTokenType.NewLine, TokenType.NewLine, ""),
            };

            // return parsed section
            return result;
        }

        /// <summary>
        /// Used by parser to create sections
        /// </summary>
        internal Section(EvaluatedSymbol<SectionType> type, Symbol parameter)
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

        /// <summary>
        /// Symbol that represents section type, evaluated as <see cref="SectionType"/>
        /// </summary>
        public EvaluatedSymbol<SectionType> Type { get; }

        /// <summary>
        /// Symbol that represents section parameter, can be null.
        /// </summary>
        public Symbol Parameter { get; }

        /// <summary>
        /// Section name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Key/Value pair of properties
        /// </summary>
        public Dictionary<EvaluatedSymbol<KeyName>, Symbol> Pairs { get; } = new();

        /// <summary>
        /// Tokens for this section, used by config file writer
        /// </summary>
        internal Token[] Tokens { get; set; }

        /// <summary>
        /// Tokens before this section, to keep comments and whitespaces between sections after changing a config file
        /// </summary>
        internal Token[] PreTokens { get; set; }

        /// <summary>
        /// Find and return first property value with <see cref="KeyName.TitleBarColor"/> as its key, evaluated to <see cref="TitlebarColorMode"/>.
        /// </summary>
        public EvaluatedSymbol<TitlebarColorMode> GetTitleBarColor()
        {
            return (EvaluatedSymbol<TitlebarColorMode>)
                Pairs.First(pair => pair.Key.Value == KeyName.TitleBarColor).Value;
        }

        /// <summary>
        /// Find and return first property value with <see cref="KeyName.BackdropPreference"/> as its key, evaluated to <see cref="BackdropType"/>.
        /// </summary>
        public EvaluatedSymbol<BackdropType> GetBackdropPreference()
        {
            return (EvaluatedSymbol<BackdropType>)
                Pairs.First(pair => pair.Key.Value == KeyName.BackdropPreference).Value;
        }

        /// <summary>
        /// Find and return first property value with <see cref="KeyName.ExtendFrameToClientArea"/> as its key, evaluated to <see cref="bool"/>.
        /// </summary>
        public EvaluatedSymbol<bool> GetExtendFrameIntoClientArea()
        {
            return (EvaluatedSymbol<bool>)
                Pairs.First(pair => pair.Key.Value == KeyName.ExtendFrameToClientArea).Value;
        }
    }
}
