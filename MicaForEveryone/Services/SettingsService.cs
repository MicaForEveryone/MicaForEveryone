using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.Win32;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Models;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class SettingsService : ISettingsService, IDisposable
    {
        private const string SettingsRegistryKey = @"Software\MicaForEveryone";
        private const string ConfigFilePathKey = "config file path";
        private const string FileWatcherKey = "file watcher state";

        private static string UwpGetDefaultConfigFilePath()
        {
            var appData = ApplicationData.Current.LocalFolder.Path;
            return Path.Join(appData, "MicaForEveryone.conf");
        }

        private static string Win32GetDefaultConfigFilePath()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Join(appData, "Mica For Everyone", "MicaForEveryone.conf");
        }

        private readonly RegistryKey _key;

        public SettingsService(IConfigFile configFile)
        {
            ConfigFile = configFile;
            ConfigFile.FileChanged += ConfigFile_FileChanged;

            _key = Registry.CurrentUser.OpenSubKey(SettingsRegistryKey, true) ??
                Registry.CurrentUser.CreateSubKey(SettingsRegistryKey, true);
        }

        public IConfigFile ConfigFile { get; }

        public IRule[] Rules { get; private set; } = new IRule[0];

        public void Load()
        {
            var configPath = _key.GetValue(ConfigFilePathKey) as string;

            if (configPath == null)
            {
                configPath = Application.IsPackaged ?
                    UwpGetDefaultConfigFilePath() :
                    Win32GetDefaultConfigFilePath();
            }

            if (!File.Exists(configPath))
            {
                var appFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                var defaultConfigPath = Path.Join(appFolder, "MicaForEveryone.conf");
                if (File.Exists(defaultConfigPath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(configPath));
                    File.Copy(defaultConfigPath, configPath);
                }
            }

            ConfigFile.FilePath = configPath;

            var watcherState = true;
            bool.TryParse(_key.GetValue(FileWatcherKey)?.ToString(), out watcherState);
            ConfigFile.IsFileWatcherEnabled = watcherState;
        }

        public void Save()
        {
            _key.SetValue(ConfigFilePathKey, ConfigFile.FilePath);
            _key.SetValue(FileWatcherKey, ConfigFile.IsFileWatcherEnabled);
            _key.Flush();
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
                        await LoadRulesAsync();
                        break;

                    case SettingsChangeType.ConfigFileWatcherStateChanged:
                    case SettingsChangeType.ConfigFileReloaded:
                        Save();
                        break;
                }

                Changed?.Invoke(this, new SettingsChangedEventArgs(type, rule));
            });
        }

        public void Dispose()
        {
            _key.Dispose();
        }

        private async void ConfigFile_FileChanged(object? sender, EventArgs e)
        {
            await LoadRulesAsync();
        }

        public event EventHandler<SettingsChangedEventArgs>? Changed;
    }
}
