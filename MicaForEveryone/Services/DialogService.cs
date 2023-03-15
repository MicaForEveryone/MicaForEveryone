using MicaForEveryone.Core.Interfaces;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Views;
using MicaForEveryone.Win32;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class DialogService : IDialogService
    {
        private IAppLifeTimeService _lifetimeService;

        public DialogService(IAppLifeTimeService lifetimeService)
        {
            _lifetimeService = lifetimeService;
        }

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
            using var app = new App();
            dialog.Activate();
            dialog.ShowWindow();
            app.AddWindow(dialog);
            dialog.Destroy += (_, _) =>
            {
                app.Exit();
            };
            app.Run();
        }

        public Dialog ShowErrorDialog(Window? parent, object title, object content, int width, int height)
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
            if (_lifetimeService.IsViewServiceRunning)
            {
                ShowDialog(null, dialog);
            }
            else
            {
                RunDialog(dialog);
            }
        }
    }
}
