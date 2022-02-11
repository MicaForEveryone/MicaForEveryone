using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Models;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class SettingsService : ISettingsService
    {
        private const string ConfigFilePathKey = "config file path";
        private const string FileWatcherKey = "file watcher state";

        private readonly ISettingsContainer _container;

        public SettingsService(IConfigFile configFile, ISettingsContainer container)
        {
            _container = container;
            ConfigFile = configFile;
            ConfigFile.FileChanged += ConfigFile_FileChanged;
        }

        public IConfigFile ConfigFile { get; }

        public IRule[] Rules { get; private set; } = new IRule[0];

        public void Load()
        {
            ConfigFile.FilePath = _container.GetValue(ConfigFilePathKey) as string;

            if (bool.TryParse(_container.GetValue(FileWatcherKey)?.ToString(), out var watcherState))
            {
                ConfigFile.IsFileWatcherEnabled = watcherState;
            }
            else
            {
                ConfigFile.IsFileWatcherEnabled = true;
            }
        }

        public void Save()
        {
            _container.SetValue(ConfigFilePathKey, ConfigFile.FilePath);
            _container.SetValue(FileWatcherKey, ConfigFile.IsFileWatcherEnabled);
            _container.Flush();
        }

        public async Task LoadRulesAsync()
        {
            var rules = (await ConfigFile.LoadAsync()).ToList();

            // add an empty global rule when no global rule provided
            if (rules.All(rule => rule is not GlobalRule))
            {
                var globalRule = new GlobalRule();
                rules.Add(globalRule);
                ConfigFile.Parser.AddRule(globalRule);
            }

            // Check for duplicates
            var duplicates = rules.GroupBy(x => x.Name).Where(group => group.Skip(1).Any());
            if (duplicates.Any())
            {
                var duplicateRuleNames = duplicates.Select(x => x.Key);
                throw new Exception($"Duplicate rules found: {string.Join(", ", duplicateRuleNames)}");
            }

            Rules = rules.ToArray();
            RaiseChanged(SettingsChangeType.ConfigFileReloaded, null);
        }

        public void RaiseChanged(SettingsChangeType type, IRule? rule)
        {
            Task.Run(async () =>
            {
                switch (type)
                {
                    case SettingsChangeType.RuleAdded:
                        ConfigFile.Parser.AddRule(rule);
                        Rules = ConfigFile.Parser.Rules;
                        await ConfigFile.SaveAsync();
                        break;

                    case SettingsChangeType.RuleRemoved:
                        ConfigFile.Parser.RemoveRule(rule);
                        Rules = ConfigFile.Parser.Rules;
                        await ConfigFile.SaveAsync();
                        break;

                    case SettingsChangeType.RuleChanged:
                        ConfigFile.Parser.SetRule(rule);
                        await ConfigFile.SaveAsync();
                        break;

                    case SettingsChangeType.ConfigFilePathChanged:
                        await ConfigFile.InitializeAsync();
                        await LoadRulesAsync();
                        Save();
                        break;

                    case SettingsChangeType.ConfigFileWatcherStateChanged:
                        Save();
                        break;

                    case SettingsChangeType.ConfigFileReloaded:
                        break;
                }

                Changed?.Invoke(this, new SettingsChangedEventArgs(type, rule));
            });
        }

        public void Dispose()
        {
            _container.Dispose();
        }

        private async void ConfigFile_FileChanged(object? sender, EventArgs e)
        {
            await LoadRulesAsync();
        }

        public event EventHandler<SettingsChangedEventArgs>? Changed;
    }
}
