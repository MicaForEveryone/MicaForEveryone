using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.ViewModels
{
    internal class RuleSettingsViewModel : BaseViewModel, IRuleSettingsViewModel
    {
        private readonly ISettingsService _settingsService;

        private IRule _rule;

        public RuleSettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public BackdropType BackdropType
        {
            get => _rule.BackdropPreference;
            set
            {
                if (_rule.BackdropPreference != value)
                {
                    _rule.BackdropPreference = value;
                    OnPropertyChanged();
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, _rule);
                }
            }
        }

        public TitlebarColorMode TitlebarColor
        {
            get => _rule.TitleBarColor;
            set
            {
                if (_rule.TitleBarColor != value)
                {
                    _rule.TitleBarColor = value;
                    OnPropertyChanged();
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, _rule);
                }
            }
        }

        public bool ExtendFrameIntoClientArea
        {
            get => _rule.ExtendFrameIntoClientArea;
            set
            {
                if (_rule.ExtendFrameIntoClientArea != value)
                {
                    _rule.ExtendFrameIntoClientArea = value;
                    OnPropertyChanged();
                    _settingsService.CommitChangesAsync(SettingsChangeType.RuleChanged, _rule);
                }
            }
        }

        public ISettingsViewModel ParentViewModel { get; set; }

        public object Rule => _rule;

        public void InitializeData(object data)
        {
            if (data is IRule rule)
            {
                _rule = rule;
            }
        }
    }
}
