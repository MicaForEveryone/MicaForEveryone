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
            var rules = ConfigSource.GetRules().ToList();
            if (rules.All(rule => rule is not GlobalRule))
            {
                rules.Add(new GlobalRule());
            }
            Rules = rules.ToArray();
        }

        public async Task SaveAsync()
        {
            foreach (var rule in Rules)
            {
                ConfigSource.SetRule(rule);
            }
            await ConfigSource.SaveAsync();
        }
    }
}
