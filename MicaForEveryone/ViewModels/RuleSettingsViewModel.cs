using System.Windows.Input;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI.ViewModels;

namespace MicaForEveryone.ViewModels
{
    internal class RuleSettingsViewModel : BaseViewModel, IRuleSettingsViewModel
    {
        private readonly IConfigService _configService;

        private readonly RelyCommand _saveCommand;

        private BackdropType _backdropType;
        private TitlebarColorMode _titlebarColorMode;
        private bool _extendFrameIntoClientArea;

        public RuleSettingsViewModel(IConfigService configService)
        {
            _configService = configService;

            _saveCommand = new RelyCommand(Save, CanSave);
        }

        public IRule Rule { get; set; }

        public BackdropType BackdropType
        {
            get => _backdropType;
            set
            {
                SetProperty(ref _backdropType, value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public TitlebarColorMode TitlebarColor
        {
            get => _titlebarColorMode;
            set
            {
                SetProperty(ref _titlebarColorMode, value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool ExtendFrameIntoClientArea
        {
            get => _extendFrameIntoClientArea;
            set
            {
                SetProperty(ref _extendFrameIntoClientArea, value);
                _saveCommand.RaiseCanExecuteChanged();
            }
        }

        public ISettingsViewModel ParentViewModel { get; set; }

        public ICommand SaveCommand => _saveCommand;

        public void InitializeData(object data)
        {
            if (data is IRule rule)
            {
                Rule = rule;
                BackdropType = Rule.BackdropPreference;
                TitlebarColor = Rule.TitlebarColor;
                ExtendFrameIntoClientArea = Rule.ExtendFrameIntoClientArea;
            }
        }

        private async void Save(object parameter)
        {
            Rule.BackdropPreference = BackdropType;
            Rule.TitlebarColor = TitlebarColor;
            ExtendFrameIntoClientArea = ExtendFrameIntoClientArea;

            _configService.RaiseChanged();
            await _configService.SaveAsync();
        }

        private bool CanSave(object parameter)
        {
            return BackdropType != Rule.BackdropPreference ||
                TitlebarColor != Rule.TitlebarColor ||
                ExtendFrameIntoClientArea != Rule.ExtendFrameIntoClientArea;
        }
    }
}
