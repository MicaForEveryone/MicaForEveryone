using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;

namespace MicaForEveryone.Models
{
    internal class ConfigFile : IConfigSource
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

        public void OpenInEditor()
        {
            Process.Start(_filePath);
        }

        public IEnumerable<IRule> GetRules()
        {
            if (_configDocument == null)
                throw new Exception("Config document not loaded.");

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

        public void SetRule(IRule rule)
        {
            if (_configDocument == null)
                throw new Exception("Config document not loaded.");

            var target = _configDocument.Sections.FirstOrDefault(
                section => section.Name == rule.Name);

            if (target == null)
            {
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

                _configDocument.AddNewSection(type, parameter, pairs);
            }
            else
            {
                OverrideToSectionFromRule(target, rule);
            }
        }

        public async Task LoadAsync()
        {
            if (!File.Exists(_filePath))
            {
                _configDocument = Document.Empty;
                return;
            }
            using var reader = File.OpenText(_filePath);
            _configDocument = await Document.ParseAsync(reader);
        }

        public async Task SaveAsync()
        {
            using var file = File.Open(_filePath, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(file);
            await _configDocument.SaveAsync(writer);
        }
    }
}
