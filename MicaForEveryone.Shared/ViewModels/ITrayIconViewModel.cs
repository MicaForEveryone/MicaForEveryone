using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
    public interface ITrayIconViewModel : INotifyPropertyChanged
    {
        bool SystemBackdropIsSupported { get; }
        BackdropType BackdropType { get; set; }
        TitlebarColorMode TitlebarColor { get; set; }
        bool ExtendFrameIntoClientArea { get; set; }

        ICommand ExitCommand { get; }
        ICommand ReloadConfigCommand { get; }
        ICommand ChangeTitlebarColorModeCommand { get; }
        ICommand ChangeBackdropTypeCommand { get; }
        ICommand EditConfigCommand { get; }
        ICommand OpenSettingsCommand { get; }

        void InitializeApp(object sender);
        void SaveConfig();
        void RematchRules();
        void ReloadConfig();
        void ShowContextMenu(Point offset, Rectangle notifyIconRect);
        void ShowTipPopup(Rectangle notifyIconRect);
        void HideTipPopup();
    }
}
