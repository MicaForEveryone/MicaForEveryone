using System;
using Windows.UI.Xaml;

using MicaForEveryone.Models;
using MicaForEveryone.Rules;

namespace MicaForEveryone
{
    internal partial class App
    {
        private readonly RuleHandler _ruleHandler = new();

        private void InitializeRuleHandler()
        {
            // get config file path
            var args = Environment.GetCommandLineArgs();
            var filePath = args.Length > 1 ? args[1] : "MicaForEveryone.conf";
            // read config file and parse rules
            _ruleHandler.ConfigSource = new ConfigFile(filePath);
            _ruleHandler.LoadConfig();
            // get system theme
            SetSystemColorMode();
            // apply rules to all open windows
            _ruleHandler.MatchAndApplyRuleToAllWindows();
        }

        private void SetSystemColorMode()
        {
            _ruleHandler.SystemTitlebarColorMode = _uwpApp.RequestedTheme switch
            {
                ApplicationTheme.Light => TitlebarColorMode.Light,
                ApplicationTheme.Dark => TitlebarColorMode.Dark,
                _ => TitlebarColorMode.Default,
            };
        }
    }
}