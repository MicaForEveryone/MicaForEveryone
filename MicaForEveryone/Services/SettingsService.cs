using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Config;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class SettingsService : ISettingsService
    {
        private const string ConfigFilePathKey = "config file path";
        private const string FileWatcherKey = "file watcher state";
        private const string LanguageKey = "language";

        private readonly ISettingsContainer _container;
        private readonly ILanguageService _languageService;

        public SettingsService(IConfigFile configFile, ISettingsContainer container, ILanguageService languageService)
        {
            ConfigFile = configFile;
            _container = container;
            _languageService = languageService;

            ConfigFile.FileChanged += ConfigFile_FileChanged;
        }

        public IConfigFile ConfigFile { get; }

        public IRule[] Rules { get; private set; } = new IRule[0];

        public void Load()
        {
            var languageTag = _container.GetValue(LanguageKey) as string;
            var language = _languageService.SupportedLanguages.FirstOrDefault(
                                language => language.LanguageTag == languageTag);
            if (language != null)
            {
                _languageService.SetLanguage(language);
            }

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
            _container.SetValue(LanguageKey, _languageService.CurrentLanguage.LanguageTag);
            _container.SetValue(ConfigFilePathKey, ConfigFile.FilePath);
            _container.SetValue(FileWatcherKey, ConfigFile.IsFileWatcherEnabled);
            _container.Flush();
        }

        public async Task LoadRulesAsync()
        {
            var rules = new List<IRule>();

            try
            {
                rules.AddRange(await ConfigFile.LoadAsync());
            }
            catch (ParserError error)
            {
                Program.CurrentApp.Dispatcher.Enqueue(() =>
                {
                    var ctx = Program.CurrentApp.Container;
                    var dialogService = ctx.GetService<IDialogService>();
                    var resources = ResourceLoader.GetForCurrentView();
                    var title = resources.GetString("ConfigFileError/Header");
                    var body = string.Format(resources.GetString("ConfigFileError/Content"), error.Message);
                    var dialog = dialogService?.ShowErrorDialog(null, title, body, 475, 320);
                    if (dialog != null)
                    {
                        dialog.Destroy += (sender, args) =>
                        {
                            var viewService = ctx.GetService<IViewService>();
                            viewService?.MainWindow.ViewModel.EditConfigCommand.Execute(null);
                        };
                    }
                });

                return;
            }

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
            await CommitChangesAsync(SettingsChangeType.ConfigFileReloaded, null);
        }

        public async Task CommitChangesAsync(SettingsChangeType type, IRule? rule)
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
                    Save();
                    await ConfigFile.InitializeAsync();
                    await LoadRulesAsync();
                    break;

                case SettingsChangeType.ConfigFileWatcherStateChanged:
                    Save();
                    break;

                case SettingsChangeType.ConfigFileReloaded:
                    break;
            }

            Changed?.Invoke(this, new SettingsChangedEventArgs(type, rule));
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
