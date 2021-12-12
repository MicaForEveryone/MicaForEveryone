using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace MicaForEveryone
{
    internal static class Program
    {
        private static User32.HWINEVENTHOOK hook;
        private static NotifyIcon notifyIcon;
        private static bool darkMode;
        private static bool forceLightMode;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            if (Environment.OSVersion.Version.Build < 22000) {
                MessageBox.Show("This app requires Windows 11.", "Error", MessageBoxButtons.OK);
                Environment.Exit(1);
                return;
            }

            InitializeComponents();
            Application.Run();
        }

        private static void InitializeComponents()
        {
            // set application exit handler
            Application.ApplicationExit += OnApplicationExit;

            // set window hook
            hook = User32.SetWinEventHook(
                User32.EventConstants.EVENT_OBJECT_CREATE,
                User32.EventConstants.EVENT_OBJECT_CREATE,
                HINSTANCE.NULL,
                WinEventHookCallback,
                0,
                0,
                User32.WINEVENT.WINEVENT_OUTOFCONTEXT);

            // initialize tray icon
            notifyIcon = new NotifyIcon
            {
                Text = "Mica for Everyone",
                Icon = User32.LoadIcon(HINSTANCE.NULL, User32.IDI_APPLICATION).ToIcon(),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip
                {
                    Items =
                    {
                        new ToolStripMenuItem("Refresh all Windows", null, (sender, args) => RefreshAllWindows()),
                        new ToolStripMenuItem("Dark Mode", null, (sender, args) => darkMode = !darkMode) { CheckOnClick = true },
                        new ToolStripMenuItem("Force Light Mode", null, (sender, args) => forceLightMode = !forceLightMode) { CheckOnClick = true },
                        new ToolStripMenuItem("Exit", null, (sender, args) => Application.Exit()),
                    }
                }
            };

            // enable mica on already running apps
            RefreshAllWindows();
        }

        private static void HandleWindow(HWND hwnd)
        {
            #if DEBUG
            Debug.WriteLine($"Enabling Mica for {hwnd.GetWindowTitle()} ({hwnd.GetClassName()}, {hwnd.GetWindowProcessName()})");
            #endif
            try
            {
                hwnd.SetMica(true).ThrowIfFailed();
                if (darkMode)
                    hwnd.SetImmersiveDarkMode(true).ThrowIfFailed();
                else if (forceLightMode)
                    hwnd.SetImmersiveDarkMode(false).ThrowIfFailed();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to enable Mica for {hwnd.GetWindowTitle()} ({hwnd.GetWindowProcessName()}), Error: {e.Message}");
            }
        }

        private static void RefreshAllWindows()
        {
            User32.EnumWindows((hwnd, param) =>
            {
                if (hwnd.HasCaption())
                    HandleWindow(hwnd);
                return true;
            }, IntPtr.Zero);
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            if (!hook.IsNull)
            {
                User32.UnhookWinEvent(hook);
            }
        }

        private static void WinEventHookCallback(User32.HWINEVENTHOOK hwineventhook, uint winevent, HWND hwnd, int idobject, int idchild, uint ideventthread, uint dwmseventtime)
        {
            #if DEBUG
            Debug.WriteLine($"Event: {winevent}, {hwnd.GetWindowTitle()} ({hwnd.GetClassName()}, {hwnd.GetWindowProcessName()}), {idobject}, {idchild} ");
            #endif
            if (winevent == User32.EventConstants.EVENT_OBJECT_CREATE && hwnd.HasCaption())
            {
                HandleWindow(hwnd);
            }
        }
    }
}
