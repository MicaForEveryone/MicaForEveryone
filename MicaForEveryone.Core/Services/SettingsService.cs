using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Core.Models;
using XclParser;

namespace MicaForEveryone.Core.Services
{
    public class SettingsService : ISettingsService
    {
        private const string ConfigFilePathKey = "config file path";
        private const string FileWatcherKey = "file watcher state";
        //private const string LanguageKey = "language";
        private const string TrayIconVisibilityKey = "tray icon visibility";

        private readonly ISettingsContainer _container;
        //private readonly ILanguageService _languageService;
        private bool _trayIconVisibility = true;

        public SettingsService(IConfigFile configFile, ISettingsContainer container)
        {
            ConfigFile = configFile;
            _container = container;
            //_languageService = languageService;

            ConfigFilePathChanged += SettingsService_ConfigFilePathChanged;
            ConfigFile.FileChanged += ConfigFile_FileChanged;
        }

        // properties

        public IConfigFile ConfigFile { get; }

        public IRule[] Rules { get; private set; } = Array.Empty<IRule>();

        public bool TrayIconVisibility
        {
            get => _trayIconVisibility;
            set
            {
                if (_trayIconVisibility == value)
                    return;

                _trayIconVisibility = value;
                _container.SetValue(TrayIconVisibilityKey, TrayIconVisibility);

                TrayIconVisibilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string? ConfigFilePath
        {
            get => ConfigFile.FilePath;
            set
            {
                if (ConfigFile.FilePath == value)
                    return;

                ConfigFile.FilePath = value;
                _container.SetValue(ConfigFilePathKey, ConfigFile.FilePath);

                ConfigFilePathChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsFileWatcherEnabled
        {
            get => ConfigFile.IsFileWatcherEnabled;
            set
            {
                if (ConfigFile.IsFileWatcherEnabled == value)
                    return;

                ConfigFile.IsFileWatcherEnabled = value;
                _container.SetValue(FileWatcherKey, ConfigFile.IsFileWatcherEnabled);

                ConfigFileWatcherStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        //public Language Language
        //{
        //    get => _languageService.CurrentLanguage;
        //    set
        //    {
        //        if (_languageService.CurrentLanguage == value)
        //            return;

        //        _languageService.SetLanguage(value);
        //        _container.SetValue(LanguageKey, _languageService.CurrentLanguage.LanguageTag);

        //        LanguageChanged?.Invoke(this, EventArgs.Empty);
        //    }
        //}

        // initializer

        public void Load()
        {
            //var languageTag = _container.GetValue(LanguageKey) as string;
            //var language = _languageService.SupportedLanguages.FirstOrDefault(
            //                    language => language.LanguageTag == languageTag);
            //if (language != null)
            //{
            //    _languageService.SetLanguage(language);
            //}

            ConfigFile.FilePath = _container.GetValue(ConfigFilePathKey) as string;

            ConfigFile.IsFileWatcherEnabled = !bool.TryParse(_container.GetValue(FileWatcherKey)?.ToString(), out var watcherState) || watcherState;
            _trayIconVisibility = !bool.TryParse(_container.GetValue(TrayIconVisibilityKey)?.ToString(), out var trayIconVisibility) || trayIconVisibility;
        }

        public async Task InitializeAsync()
        {
            Load();
            await ConfigFile.InitializeAsync();
            await LoadRulesAsync();
        }

        // rules

        public async Task AddRuleAsync(IRule rule)
        {
            ConfigFile.Parser.AddRule(rule);
            Rules = ConfigFile.Parser.Rules;
            await ConfigFile.SaveAsync();
            RuleAdded?.Invoke(this, new RulesChangeEventArgs(rule));
        }

        public async Task RemoveRuleAsync(IRule rule)
        {
            ConfigFile.Parser.RemoveRule(rule);
            Rules = ConfigFile.Parser.Rules;
            await ConfigFile.SaveAsync();
            RuleRemoved?.Invoke(this, new RulesChangeEventArgs(rule));
        }

        public async Task UpdateRuleAsync(IRule rule)
        {
            ConfigFile.Parser.SetRule(rule);
            await ConfigFile.SaveAsync();
            RuleChanged?.Invoke(this, new RulesChangeEventArgs(rule));
        }

        public async Task ResetRulesAsync()
        {
            await ConfigFile.ResetAsync();
            await LoadRulesAsync();
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
                // TODO: show error
                //Program.CurrentApp.Dispatcher.Enqueue(() =>
                //{
                //    var ctx = Program.CurrentApp.Container;
                //    var dialogService = ctx.GetService<IDialogService>();
                //    var resources = ResourceLoader.GetForCurrentView();
                //    var title = resources.GetString("ConfigFileError/Header");
                //    var body = string.Format(resources.GetString("ConfigFileError/Content"), error.Message);
                //    var dialog = dialogService?.ShowErrorDialog(null, title, body, 475, 320);
                //    if (dialog != null)
                //    {
                //        dialog.Destroy += (sender, args) =>
                //        {
                //            var viewService = ctx.GetService<IViewService>();
                //            viewService?.MainWindow.ViewModel.EditConfigCommand.Execute(null);
                //        };
                //    }
                //});

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
            var duplicates = rules.GroupBy(x => x.Name).Where(group => group.Skip(1).Any()).ToArray();
            if (duplicates.Any())
            {
                var duplicateRuleNames = duplicates.Select(x => x.Key);
                throw new Exception($"Duplicate rules found: {string.Join(", ", duplicateRuleNames)}");
            }

            Rules = rules.ToArray();
            ConfigFileReloaded?.Invoke(this, EventArgs.Empty);
        }

        // IDisposable

        public void Dispose()
        {
            _container.Dispose();
        }

        // event handlers

        private async void ConfigFile_FileChanged(object? sender, EventArgs e)
        {
            await LoadRulesAsync();
        }

        private async void SettingsService_ConfigFilePathChanged(object? sender, EventArgs e)
        {
            await ConfigFile.InitializeAsync();
            await LoadRulesAsync();
        }

        public event EventHandler<RulesChangeEventArgs>? RuleAdded;
        public event EventHandler<RulesChangeEventArgs>? RuleRemoved;
        public event EventHandler<RulesChangeEventArgs>? RuleChanged;
        public event EventHandler? ConfigFilePathChanged;
        public event EventHandler? ConfigFileWatcherStateChanged;
        public event EventHandler? ConfigFileReloaded;
        public event EventHandler? TrayIconVisibilityChanged;
        //public event EventHandler? LanguageChanged;
    }
}
