using MicaForEveryone.Win32;

namespace MicaForEveryone.Interfaces
{
    public interface IDialogService
    {
        void ShowDialog(Window parent, Dialog dialog);

        void RunDialog(Dialog dialog);

        void ShowErrorDialog(Window parent, object title, object content, int width, int height);

        void RunErrorDialog(object title, object content, int width, int height);
    }
}
