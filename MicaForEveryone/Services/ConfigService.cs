using System;
using System.Linq;
using System.Threading.Tasks;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.Services
{
    internal class ConfigService : IConfigService
    {
        public ConfigService(IConfigSource source)
        {
            ConfigSource = source;
        }

        public IConfigSource ConfigSource { get; }

        public IRule[] Rules { get; private set; }

        public async Task LoadAsync()
        {
            await ConfigSource.LoadAsync();

            PopulateRules();
        }

        public async Task SaveAsync()
        {
            foreach (var rule in Rules)
            {
                ConfigSource.SetRule(rule);
            }
            await ConfigSource.SaveAsync();
        }

        public void PopulateRules()
        {
            var rules = ConfigSource.GetRules().ToList();

            // add an empty global rule when no global rule provided
            if (rules.All(rule => rule is not GlobalRule))
            {
                rules.Add(new GlobalRule());
            }

            // Check for duplicates
            var duplicates = rules.GroupBy(x => x.Name)
                .Where(group => group.Skip(1).Any());
            if (duplicates.Any())
            {
                var duplicateRuleNames = duplicates.Select(x => x.Key);
                throw new Exception($"There are duplicate rules found in config file.{Environment.NewLine}{Environment.NewLine}List of duplicate rules:{Environment.NewLine}{string.Join(Environment.NewLine, duplicateRuleNames)}");
            }

            Rules = rules.ToArray();
            RaiseChanged();
        }

        public void RaiseChanged()
        {
            Updated?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Updated;
    }
}
