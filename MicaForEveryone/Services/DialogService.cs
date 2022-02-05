using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Views;

namespace MicaForEveryone.Services
{
    internal class DialogService : IDialogService
    {
        public void ShowDialog(Window parent, Dialog dialog)
        {
            dialog.Parent = parent.Handle;
            dialog.Activate();
            dialog.CenterToDesktop();
            dialog.UpdatePosition();
            dialog.Show();
            dialog.SetForegroundWindow();
        }

        public void RunDialog(Dialog dialog)
        {
            dialog.Destroy += (sender, args) =>
            {
                Program.CurrentApp.Exit();
            };
            dialog.Activate();
            dialog.CenterToDesktop();
            dialog.UpdatePosition();
            dialog.Show();
            Program.CurrentApp.Run(dialog);
        }

        public void ShowErrorDialog(Window parent, object title, object content, int width, int height)
        {
            var dialog = new ErrorDialog
            {
                Width = width,
                Height = height,
            };
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.SetTitle(title);
            dialog.SetContent(content);
            ShowDialog(parent, dialog);
        }

        public void RunErrorDialog(object title, object content, int width, int height)
        {
            using var dialog = new ErrorDialog
            {
                Width = width,
                Height = height,
            };
            dialog.SetTitle(title);
            dialog.SetContent(content);
            RunDialog(dialog);
        }
    }
}
