using System;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

using static Vanara.PInvoke.User32;

using MicaForEveryone.ViewModels;
using MicaForEveryone.UWP;
using System.Diagnostics;

namespace MicaForEveryone.Xaml
{
    public class XamlApplication : Application
    {
        private WindowsXamlManager _xamlManager;

        public XamlApplication()
        {
            BeforeRun += XamlApplication_BeforeRun;
            BeforeExit += XamlApplication_BeforeExit;
            BeforeTranslateMessage += XamlApplication_BeforeTranslateMessage;
        }

        private void XamlApplication_BeforeRun(object sender, EventArgs e)
        {
            _xamlManager = WindowsXamlManager.InitializeForCurrentThread();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void XamlApplication_BeforeExit(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
        }

        private void XamlApplication_BeforeTranslateMessage(Win32.Window window, ref MSG message, ref bool processed)
        {
            if (window is XamlWindow xamlWindow)
            {
                processed = xamlWindow.GetXamlWindowInterop().PreTranslateMessage(ref message);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var errorContent = new ContentDialogView
            {
                ViewModel =
                {
                    Title = "Error",
                    Content = "An unhandled exception occured:\n" + args.ExceptionObject.ToString(),
                    IsPrimaryButtonEnabled = true,
                    PrimaryButtonContent = "OK",
                },
            };
            using var errorDialog = new XamlDialog(errorContent)
            {
                ClassName = "Dialog",
                Title = "Mica For Everyone",
                Width = 576,
                Height = 720,
                Style = WindowStyles.WS_DLGFRAME,
            };
            errorContent.ViewModel.PrimaryCommand = new RelyCommand(_ =>
            {
                errorDialog.Close();
                Exit();
            });
            errorDialog.CenterToDesktop();
            errorDialog.Activate();
            errorDialog.Show();

            while (GetMessage(out var msg, HWND.NULL, 0, 0))
            {
                if (!errorDialog.GetXamlWindowInterop().PreTranslateMessage(msg))
                {
                    TranslateMessage(msg);
                    DispatchMessage(msg);
                }
            }

            Exit();
        }
    }
}
