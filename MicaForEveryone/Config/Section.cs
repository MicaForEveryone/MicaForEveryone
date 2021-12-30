using System;
using System.Collections.Generic;
using System.Linq;

using MicaForEveryone.Models;
using MicaForEveryone.Rules;

namespace MicaForEveryone.Config
{
    public class Section
    {
        public Section(EvaluatedSymbol<SectionType> type, Symbol parameter)
        {
            Type = type;
            Parameter = parameter;
        }

        public EvaluatedSymbol<SectionType> Type { get; }

        public Symbol Parameter { get; }

        public Dictionary<EvaluatedSymbol<KeyName>, Symbol> Pairs { get; } = new();

        public void OverrideSection(IRule source)
        {
            GetTitleBarColor().Value = source.TitlebarColor;
            GetBackdropPreference().Value = source.BackdropPreference;
            GetExtendFrameIntoClientArea().Value = source.ExtendFrameIntoClientArea;
        }

        public void OverrideRule(IRule target)
        {
            target.TitlebarColor = GetTitleBarColor().Value;
            target.BackdropPreference = GetBackdropPreference().Value;
            target.ExtendFrameIntoClientArea = GetExtendFrameIntoClientArea().Value;
        }

        public EvaluatedSymbol<TitlebarColorMode> GetTitleBarColor()
        {
            return (EvaluatedSymbol<TitlebarColorMode>)Pairs
                .First(pair => pair.Key.Value == KeyName.TitleBarColor).Value;
        }

        public EvaluatedSymbol<BackdropType> GetBackdropPreference()
        {
            return (EvaluatedSymbol<BackdropType>)Pairs
                .First(pair => pair.Key.Value == KeyName.BackdropPreference).Value;
        }

        public EvaluatedSymbol<bool> GetExtendFrameIntoClientArea()
        {
            return (EvaluatedSymbol<bool>)Pairs
                .First(pair => pair.Key.Value == KeyName.ExtendFrameToClientArea).Value;
        }

        public GlobalRule ToGlobalRule()
        {
            if (Parameter != null)
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

        public IRule ToRule()
        {
            return Type.Value switch
            {
                SectionType.Global => ToGlobalRule(),
                SectionType.Process => ToProcessRule(),
                SectionType.Class => ToClassRule(),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}
