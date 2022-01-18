using MicaForEveryone.Win32;

namespace MicaForEveryone.Interfaces
{
    public interface IDialogService
    {
        void ShowDialog(NativeWindow parent, Dialog dialog);

        void RunDialog(Dialog dialog);

        void ShowErrorDialog(NativeWindow parent, object title, object content, int width, int height);

        void RunErrorDialog(object title, object content, int width, int height);
    }
}
