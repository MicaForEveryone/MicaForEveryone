using System;

using static Vanara.PInvoke.User32;

using MicaForEveryone.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Views;

namespace MicaForEveryone
{
    internal partial class App
    {
        public static RelyCommand CloseDialogCommand { get; } =
            new RelyCommand(dialog => ((Dialog)dialog).Close());

        private void ShowAboutDialog()
        {
            var dialog = new AboutDialog()
            {
                Parent = _mainWindow.Handle,
            };
            dialog.Destroy += (sender, args) =>
            {
                dialog.Dispose();
            };
            dialog.Activate();
            dialog.CenterToDesktop();
            dialog.UpdatePosition();
            dialog.Show();
            SetForegroundWindow(dialog.Handle);
        }

        private void ShowWindows11RequiredDialog()
        {
            using var dialog = new ErrorDialog
            {
                Width = 400,
                Height = 275,
            };
            dialog.SetTitle("Error!");
            dialog.SetContent("This app requires at least Windows 11 (10.0.22000.0) to work.");
            dialog.Destroy += (sender, args) =>
            {
                Exit();
            };
            dialog.Activate();
            dialog.CenterToDesktop();
            dialog.UpdatePosition();
            dialog.Show();
            Run(dialog);
        }

        private void ShowUnhandledExceptionDialog(Exception exception)
        {
            using var dialog = new ErrorDialog();
            dialog.Destroy += (sender, args) =>
            {
                Exit();
            };
            dialog.SetTitle(exception.Message);
            dialog.SetContent(exception.ToString());
            dialog.Activate();
            dialog.CenterToDesktop();
            dialog.UpdatePosition();
            dialog.Show();
            SetForegroundWindow(dialog.Handle);

            Run(dialog);
        }
    }
}