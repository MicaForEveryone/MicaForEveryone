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
    internal class ConfigFile : IConfigSource, IDisposable
    {
        private static StreamReader TryOpenFile(string filePath, int timeout = 1000)
        {
            var time = Stopwatch.StartNew();
            while (time.ElapsedMilliseconds < timeout)
            {
                try
                {
                    return File.OpenText(filePath);
                }
                catch (IOException exception)
                {
                    if (exception.HResult != -2147024864)
                        throw;
                }
            }
            throw new TimeoutException($"Failed to get a write handle to {filePath} within {timeout}ms.");
        }

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
        private readonly string _fileName;

        private Document _configDocument;
        private FileSystemWatcher _fileSystemWatcher;

        public ConfigFile(string filePath)
        {
            _filePath = filePath;
            _fileName = Path.GetFileName(filePath);
            var directoryPath = Directory.GetParent(filePath).FullName;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            _fileSystemWatcher = new FileSystemWatcher(directoryPath);
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
        }

        public void OpenInEditor()
        {
            var startInfo = new ProcessStartInfo(_filePath)
            {
                UseShellExecute = true
            };
            if (startInfo.Verbs.Contains("edit"))
            {
                startInfo.Verb = "edit";
            }
            Process.Start(startInfo);
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
            using var reader = TryOpenFile(_filePath);
            _configDocument = await Document.ParseAsync(reader);
        }

        public async Task SaveAsync()
        {
            try
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                using var file = File.Open(_filePath, FileMode.Create, FileAccess.Write);
                using var writer = new StreamWriter(file);
                await _configDocument.SaveAsync(writer);
            }
            finally
            {
                _fileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        public bool GetWatchState()
        {
            return _fileSystemWatcher.EnableRaisingEvents;
        }

        public void SetWatchState(bool state)
        {
            _fileSystemWatcher.EnableRaisingEvents = state;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name == _fileName)
                Changed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Changed;
    }
}
