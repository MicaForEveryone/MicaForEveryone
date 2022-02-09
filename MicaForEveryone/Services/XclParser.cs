using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.Services
{
    internal class XclParser : IConfigParser
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

        private static IEnumerable<IRule> ParseRules(Document config)
        {
            foreach (var section in config.Sections)
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

        private Document _configDocument;

        public IRule[] Rules { get; private set; }

        public async Task LoadAsync(StreamReader reader)
        {
            _configDocument = await Document.ParseAsync(reader);
            Rules = ParseRules(_configDocument).ToArray();
        }

        public async Task SaveAsync(StreamWriter writer)
        {
            await _configDocument.SaveAsync(writer);
        }

        public void AddRule(IRule rule)
        {
            if (_configDocument == null)
                throw new Exception("Config document not loaded.");

            if (_configDocument.Sections.Any(section => section.Name == rule.Name))
                throw new Exception("Rule already exists.");

            var type = rule switch
            {
                GlobalRule => SectionType.Global,
                ProcessRule => SectionType.Process,
                ClassRule => SectionType.Class,
                _ => throw new ArgumentOutOfRangeException(),
            };

            var parameter = rule switch
            {
                GlobalRule => null,
                ProcessRule processRule => processRule.ProcessName,
                ClassRule classRule => classRule.ClassName,
                _ => throw new ArgumentOutOfRangeException(),
            };

            var pairs = new[]
            {
                    new KeyValuePair<KeyName, object>(KeyName.BackdropPreference, rule.BackdropPreference),
                    new KeyValuePair<KeyName, object>(KeyName.TitleBarColor, rule.TitlebarColor),
                    new KeyValuePair<KeyName, object>(KeyName.ExtendFrameToClientArea, rule.ExtendFrameIntoClientArea),
                };

            _configDocument.Sections.Add(Section.Create(type, parameter, pairs));
        }

        public void SetRule(IRule rule)
        {
            var target = _configDocument.Sections.FirstOrDefault(
                section => section.Name == rule.Name);

            if (target == null)
                throw new Exception($"Rule {rule.Name} not found in config file.");

            OverrideToSectionFromRule(target, rule);
        }

        public void RemoveRule(IRule rule)
        {
            var section = _configDocument.Sections.First(section => section.Name == rule.Name);
            _configDocument.Sections.Remove(section);
        }
    }
}
