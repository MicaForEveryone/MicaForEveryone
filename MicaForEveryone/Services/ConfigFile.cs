using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.Services
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

        public ConfigFile()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                _filePath = args[1];
            }
            else
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                _filePath = Path.Join(appData, "Mica For Everyone", "MicaForEveryone.conf");
            }
        }

        public void OpenInEditor()
        {
            Process.Start(_filePath);
        }

        public IEnumerable<IRule> GetRules()
        {
            if (_configDocument == null)
                throw new Exception("Config document not loaded.");

            List<IRule> rules = new();

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
                rules.Add(rule);
            }

            // Check if there are no rules
            if (rules.Count == 0)
                throw new Exception("There must be at least one rule in the config file.");

            // Check for duplicates
            var duplicates = rules.GroupBy(x => x.Name)
                   .Where(x => x.Skip(1).Any());
            if (duplicates.Any())
            {
                // There are duplicates in the config file.
                var duplicateRuleNames = duplicates.Select(x => x.Key);
                throw new Exception($"There are duplicate rules found in config file.{Environment.NewLine}{Environment.NewLine}List of duplicate rules:{Environment.NewLine}{string.Join(Environment.NewLine, duplicateRuleNames)}");
            }
            return rules;
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
