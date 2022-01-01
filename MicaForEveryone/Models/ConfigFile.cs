using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.Rules
{
    public class ConfigFile : IConfigSource
    {
        private static void OverrideToRuleFromSection(IRule rule, Section section)
        {
            rule.TitlebarColor = section.GetTitleBarColor().Value;
            rule.BackdropPreference = section.GetBackdropPreference().Value;
            rule.ExtendFrameIntoClientArea = section.GetExtendFrameIntoClientArea().Value;
        }

        private static void OverrideToSectionFromRule(Section section, IRule rule)
        {
            section.GetTitleBarColor().Value = rule.TitlebarColor;
            section.GetBackdropPreference().Value = rule.BackdropPreference;
            section.GetExtendFrameIntoClientArea().Value = rule.ExtendFrameIntoClientArea;
        }

        private readonly string _filePath;

        private Document _configDocument;

        public ConfigFile(string filePath)
        {
            _filePath = filePath;
        }

        public IEnumerable<IRule> ParseRules()
        {
            using var reader = File.OpenText(_filePath);
            _configDocument = Document.Parse(reader);
            foreach (var section in _configDocument.Sections)
            {
                IRule rule = section.Type.Value switch
                {
                    SectionType.Global => new GlobalRule(),
                    SectionType.Process => new ProcessRule(section.Parameter.Name),
                    SectionType.Class => new ClassRule(section.Parameter.Name),
                    _ => throw new ArgumentOutOfRangeException(),
                };
                OverrideToRuleFromSection(rule, section);
                yield return rule;
            }
        }

        public void OverrideRule(IRule rule)
        {
            if (rule is not GlobalRule)
                throw new NotImplementedException();
            var target = _configDocument.Sections.First(
                section => section.Type.Value == SectionType.Global);
            OverrideToSectionFromRule(target, rule);
            using var file = File.Open(_filePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(file);
            _configDocument.Save(writer);
        }
    }
}
