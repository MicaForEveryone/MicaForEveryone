using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;
using MicaForEveryone.Views;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class DialogService : IDialogService
    {
        public void ShowDialog(Window? parent, Dialog dialog)
        {
            if (parent != null)
                dialog.Parent = parent.Handle;
            dialog.Activate();
            dialog.Destroy += (sender, args) =>
            {
                parent?.SetEnable(true);
            };
            parent?.SetEnable(false);
            dialog.ShowWindow();
            dialog.SetForegroundWindow();
        }

        public void RunDialog(Dialog dialog)
        {
            dialog.Destroy += (sender, args) =>
            {
                Program.CurrentApp.Exit();
            };
            dialog.Activate();
            dialog.ShowWindow();
            Program.CurrentApp.Run(dialog);
        }

        public Dialog ShowErrorDialog(Window parent, object title, object content, int width, int height)
        {
            var dialog = new ErrorDialog
            {
                Width = width,
                Height = height,
                ViewModel =
                {
                    Title = title,
                    Content = content,
                }
            };
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            ShowDialog(parent, dialog);
            return dialog;
        }

        public void RunErrorDialog(object title, object content, int width, int height)
        {
            using var dialog = new ErrorDialog
            {
                Width = width,
                Height = height,
                ViewModel =
                {
                    Title = title,
                    Content = content,
                }
            };
            RunDialog(dialog);
        }
    }
}
