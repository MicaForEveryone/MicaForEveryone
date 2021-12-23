using System;
using Windows.System;

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
            dialog.CenterToDesktop();
            dialog.Activate();
            dialog.Show();
            SetForegroundWindow(dialog.Handle);
        }

        private void ShowWindows11RequiredDialog()
        {
            using var errorDialog = new ErrorDialog();
            errorDialog.Height = 275;
            errorDialog.Width = 400;
            errorDialog.SetMessage("This app requires at least Windows 11 (10.0.22000.0) to work.");
            errorDialog.Destroy += (sender, args) =>
            {
                Exit();
            };
            errorDialog.CenterToDesktop();
            errorDialog.Activate();
            errorDialog.Show();
            Run(errorDialog);
        }

        private void ShowUnhandledExceptionDialog(object exception)
        {
            using var dialog = new ErrorDialog
            {
                Width = 576,
                Height = 720,
            };
            dialog.Destroy += (sender, args) =>
            {
                Exit();
            };
            dialog.SetMessage(exception.ToString());
            dialog.CenterToDesktop();
            dialog.Activate();
            dialog.Show();
            SetForegroundWindow(dialog.Handle);

            Run(dialog);
        }
    }
}