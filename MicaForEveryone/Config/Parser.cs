using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using MicaForEveryone.Rules;
using MicaForEveryone.Models;

namespace MicaForEveryone.Config
{
    public class Parser
    {
        private enum RuleType
        {
            GlobalRule,
            ProcessRule,
            ClassRule,
            Override,
        }

        private enum KeyName
        {
            TitleBarColor,
            BackdropPreference,
            ExtendFrameToClientArea,
        }

        private class Symbol
        {
            public Symbol(Token token)
            {
                Name = token.Data;
                Line = token.Line;
                Column = token.Column;
            }

            public string Name { get; }

            public int Line { get; }

            public int Column { get; }

            public RuleType EvaluateToRuleType()
            {
                return Name.ToLower() switch
                {
                    "global" => RuleType.GlobalRule,
                    "process" => RuleType.ProcessRule,
                    "class" => RuleType.ClassRule,
                    "override" => RuleType.Override,
                    _ => throw new Exception($"invalid symbol '{Name}' at {Line}:{Column}"),
                };
            }

            public KeyName EvaluateToKeyName()
            {
                return Name.ToLowerInvariant() switch
                {
                    "titlebarcolor" => KeyName.TitleBarColor,
                    "backdroppreference" => KeyName.BackdropPreference,
                    "extendframetoclientarea" => KeyName.ExtendFrameToClientArea,
                    _ => throw new Exception($"invalid symbol '{Name}' at {Line}:{Column}"),
                };
            }

            public bool EvaluateToBoolean()
            {
                return Name.ToLowerInvariant() switch
                {
                    "true" => true,
                    "false" => false,
                    "enable" => true,
                    "disable" => false,
                    "0" => true,
                    "1" => false,
                    _ => throw new Exception($"invalid symbol '{Name}' at {Line}:{Column}"),
                };
            }

            public TitlebarColorMode EvaluateToColorMode()
            {
                return Name.ToLowerInvariant() switch
                {
                    "default" => TitlebarColorMode.Default,
                    "system" => TitlebarColorMode.System,
                    "light" => TitlebarColorMode.Light,
                    "dark" => TitlebarColorMode.Dark,
                    _ => throw new Exception($"invalid symbol '{Name}' at {Line}:{Column}"),
                };
            }

            public BackdropType EvaluateToBackdropType()
            {
                return Name.ToLowerInvariant() switch
                {
                    "default" => BackdropType.Default,
                    "none" => BackdropType.None,
                    "mica" => BackdropType.Mica,
                    "acrylic" => BackdropType.Acrylic,
                    "tabbed" => BackdropType.Tabbed,
                    _ => throw new Exception($"invalid symbol '{Name}' at {Line}:{Column}"),
                };
            }

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

        private class ConfigSection
        {
            public ConfigSection(Symbol type, Symbol paramter)
            {
                Type = type;
                Parameter = paramter;
            }

            public Symbol Type { get; }
            public Symbol Parameter { get; }
            public Dictionary<Symbol, Symbol> Pairs { get; } = new();

            public void OverrideRule(IRule target)
            {
                foreach (var pair in Pairs)
                {
                    switch (pair.Key.EvaluateToKeyName())
                    {
                        case KeyName.TitleBarColor:
                            target.TitlebarColor = pair.Value.EvaluateToColorMode();
                            break;
                        case KeyName.BackdropPreference:
                            target.BackdropPreference = pair.Value.EvaluateToBackdropType();
                            break;
                        case KeyName.ExtendFrameToClientArea:
                            target.ExtendFrameIntoClientArea = pair.Value.EvaluateToBoolean();
                            break;
                    }
                }
            }

            public GlobalRule ToGlobalRule()
            {
                if (!string.IsNullOrWhiteSpace(Parameter.Name))
                    throw new Exception($"unexpected parameter for Global at {Parameter.Line}:{Parameter.Column}");
                var result = new GlobalRule();
                OverrideRule(result);
                return result;
            }

            public ProcessRule ToProcessRule()
            {
                if (string.IsNullOrWhiteSpace(Parameter.Name))
                    throw new Exception($"expected process name at {Parameter.Line}:{Parameter.Column}");
                var result = new ProcessRule(Parameter.Name);
                OverrideRule(result);
                return result;
            }

            public ClassRule ToClassRule()
            {
                if (string.IsNullOrWhiteSpace(Parameter.Name))
                    throw new Exception($"expected class name at {Parameter.Line}:{Parameter.Column}");
                var result = new ClassRule(Parameter.Name);
                OverrideRule(result);
                return result;
            }
        }

        public static IRule[] ParseFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var tokens = new Tokenizer(new Lexer(lines).Parse()).Parse();
            return new Parser(tokens).Parse();
        }

        private readonly List<ConfigSection> _result = new();
        private int _position;

        public Parser(Token[] data)
        {
            Data = data;
        }

        private Token[] Data { get; }
        private Token CurrentToken => Data[_position];

        private void NextToken()
        {
            _position++;
            if (_position >= Data.Length)
                throw new FormatException("unexpected end of file");
        }

        private void ExpectToken(TokenType expected)
        {
            if (CurrentToken.Type != expected)
                throw UnexpectedToken();
        }

        private void NextToken(TokenType expected)
        {
            NextToken();
            ExpectToken(expected);
        }

        public IRule[] Parse()
        {
            if (_result.Count > 0)
                return GetRules();

            while (_position < Data.Length)
            {
                ExpectToken(TokenType.NewSection);
                ParseSection();
                _position++;
            }

            return GetRules();
        }

        private IRule[] GetRules()
        {
            var result = new List<IRule>();
            
            foreach (var section in _result)
            {
                switch (section.Type.EvaluateToRuleType())
                {
                    case RuleType.GlobalRule:
                        if (result.Any(rule => rule is GlobalRule))
                            throw new Exception("more than one global rule");
                        result.Add(section.ToGlobalRule());
                        break;
                    case RuleType.ProcessRule:
                        result.Add(section.ToProcessRule());
                        break;
                    case RuleType.ClassRule:
                        result.Add(section.ToClassRule());
                        break;
                    case RuleType.Override:
                        section.OverrideRule(section.Parameter.MatchRule(result));
                        break;
                }
            }

            return result.ToArray();
        }

        private Exception UnexpectedToken()
        {
            return new FormatException($"unexpected token '{CurrentToken.Data}' at ({CurrentToken.Line}:{CurrentToken.Column})");
        }


        private void ParseSection()
        {
            if (CurrentToken.Type != TokenType.NewSection)
                throw UnexpectedToken();
            var type = new Symbol(CurrentToken);

            NextToken(TokenType.SectionData);
            var parameter = new Symbol(CurrentToken);

            var rule = new ConfigSection(type, parameter);

            while (_position < Data.Length)
            {
                NextToken();
                if (CurrentToken.Type == TokenType.EndSection)
                    break;

                ExpectToken(TokenType.KeyName);
                var name = new Symbol(CurrentToken);

                NextToken(TokenType.KeyValue);
                var value = new Symbol(CurrentToken);

                rule.Pairs.Add(name, value);
            }

            _result.Add(rule);
        }

    }
}
