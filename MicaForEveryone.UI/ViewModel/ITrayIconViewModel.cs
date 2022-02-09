using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.UI.ViewModels
{
    public interface ITrayIconViewModel : INotifyPropertyChanged
    {
        bool IsBackdropSupported { get; }
        bool IsMicaSupported { get; }
        bool IsImmersiveDarkModeSupported { get; }

        BackdropType BackdropType { get; set; }
        TitlebarColorMode TitlebarColor { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }

        ICommand ExitCommand { get; }
        ICommand ReloadConfigCommand { get; }
        ICommand ChangeTitlebarColorModeCommand { get; }
        ICommand ChangeBackdropTypeCommand { get; }
        ICommand EditConfigCommand { get; }
        ICommand OpenSettingsCommand { get; }

        void Initialize(object sender);
        void ShowContextMenu(Point offset, Rectangle notifyIconRect);
        void ShowTooltipPopup(Rectangle notifyIconRect);
        void HideTooltipPopup();
    }
}
