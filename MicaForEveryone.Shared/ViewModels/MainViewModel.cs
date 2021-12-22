using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private bool _systemBackdropIsSupported;
        private BackdropType _backdropType;
        private TitlebarColorMode _titlebarColor;
        private bool _extendFrameIntoClientArea;

        public bool SystemBackdropIsSupported
        {
            get => _systemBackdropIsSupported;
            set => SetProperty(ref _systemBackdropIsSupported, value);
        }

        public BackdropType BackdropType
        {
            get => _backdropType;
            set => SetProperty(ref _backdropType, value);
        }

        public TitlebarColorMode TitlebarColor
        {
            get => _titlebarColor;
            set => SetProperty(ref _titlebarColor, value);
        }

        public bool ExtendFrameIntoClientArea
        {
            get => _extendFrameIntoClientArea;
            set => SetProperty(ref _extendFrameIntoClientArea, value);
        }

        public RelyCommand ExitCommand { get; set; }

        public RelyCommand ReloadConfigCommand { get; set; }

        public RelyCommand ChangeTitlebarColorModeCommand { get; set; }

        public RelyCommand ChangeBackdropTypeCommand { get; set; }

        public RelyCommand ChangeExtendFrameIntoClientAreaCommand { get; set; }

        public RelyCommand AboutCommand { get; set; }
    }
}
