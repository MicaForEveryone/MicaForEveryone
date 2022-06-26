using Microsoft.Toolkit.Mvvm.ComponentModel;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

#nullable enable

namespace MicaForEveryone.ViewModels
{
    internal class RuleSettingsViewModel : ObservableObject, IRuleSettingsViewModel
    {
        private readonly ISettingsService _settingsService;

        public RuleSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public BackdropType BackdropType
        {
            get => Rule?.BackdropPreference ?? default;
            set
            {
                if (Rule != null && Rule.BackdropPreference != value)
                {
                    Rule.BackdropPreference = value;
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, Rule);
                }
            }
        }

        public TitlebarColorMode TitlebarColor
        {
            get => Rule?.TitleBarColor ?? default;
            set
            {
                if (Rule != null && Rule.TitleBarColor != value)
                {
                    Rule.TitleBarColor = value;
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, Rule);
                }
            }
        }

        public CornerPreference CornerPreference
        {
            get => Rule?.CornerPreference ?? default;
            set
            {
                if (Rule != null && Rule.CornerPreference != value)
                {
                    Rule.CornerPreference = value;
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, Rule);
                }
            }
        }

        public bool ExtendFrameIntoClientArea
        {
            get => Rule?.ExtendFrameIntoClientArea ?? default;
            set
            {
                if (Rule != null && Rule.ExtendFrameIntoClientArea != value)
                {
                    Rule.ExtendFrameIntoClientArea = value;
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, Rule);
                }
            }
        }

        public bool EnableBlurBehind
        {
            get => Rule?.EnableBlurBehind ?? default;
            set
            {
                if (Rule != null && Rule.EnableBlurBehind != value)
                {
                    Rule.EnableBlurBehind = value;
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, Rule);
                }
            }
        }

        public UI.ViewModels.ISettingsViewModel? ParentViewModel { get; set; }

        public IRule? Rule { get; set; }
    }
}
