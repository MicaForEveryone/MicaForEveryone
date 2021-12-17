using System;
using System.ComponentModel;
using Vanara.PInvoke;

using MicaForEveryone.Win32;
using MicaForEveryone.Rules;

namespace MicaForEveryone
{
    internal class Application : Component
    {
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.ShowErrorTaskDialog("Error: " + (e.ExceptionObject as Exception)?.Message,
                e.ExceptionObject.ToString());
        }

        private const uint WM_NOTIFYICON = User32.WM_APP + 1;

        private const ushort IDM_EXIT = 0;
        private const ushort IDM_REAPPLY = 1;
        private const ushort IDM_RELOAD_CONFIG = 2;

        private const ushort IDM_DEFAULT_THEME_MODE = 10;
        private const ushort IDM_LIGHT_THEME_MODE = 11;
        private const ushort IDM_DARK_THEME_MODE = 12;

        private const ushort IDM_SET_DEFAULT_BACKDROP = 20;
        private const ushort IDM_SET_NO_BACKDROP = 21;
        private const ushort IDM_SET_MICA_BACKDROP = 22;
        private const ushort IDM_SET_ACRYLIC_BACKDROP = 23;
        private const ushort IDM_SET_TABBED_BACKDROP = 24;

        private const ushort IDM_ENABLE_EXTEND_FRAME = 30;
        private const ushort IDM_DISABLE_EXTEND_FRAME = 31;

        private readonly IContainer _components = new Container();

        private WinEventHook _eventHook;
        private MessageWindow _window;
        private NotifyIcon _notifyIcon;

        public Application()
        {
            InitializeComponents();
        }

        public RuleHandler RuleHandler { get; } = new();

        public void Run()
        {
            // show error message for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            // hook an event to apply rules on new opened apps
            _eventHook.Hook(User32.EventConstants.EVENT_OBJECT_CREATE, User32.EventConstants.EVENT_OBJECT_CREATE);

            // activate components
            _window.Activate();
            _notifyIcon.Activate();

            // run main loop
            while (User32.GetMessage(out var msg, HWND.NULL, 0, 0))
            {
                User32.TranslateMessage(msg);
                User32.DispatchMessage(msg);
            }
        }

        private void InitializeComponents()
        {
            // initialize event hook
            _eventHook = new WinEventHook(_components);
            _eventHook.HookTriggered += OnHookTriggered;

            // initialize main window
            _window = new MessageWindow(_components)
            {
                Title = "Mica For Windows",
                Icon = User32.LoadIcon(HINSTANCE.NULL, User32.IDI_APPLICATION),
            };
            _window.Create += WindowOnCreate;
            _window.Destroy += WindowOnDestroy;
            _window.CommandInvoked += OnCommandInvoked;

            // initialize notify icon
            _notifyIcon = new NotifyIcon(_components)
            {
                Title = "Mica For Windows",
                Icon = User32.LoadIcon(HINSTANCE.NULL, User32.IDI_APPLICATION),
                Parent = _window,
                CallbackId = WM_NOTIFYICON,
            };
            _notifyIcon.ShowContextMenu += OnShowContextMenu;

            Disposed += OnDisposed;
        }

        private void WindowOnCreate(object sender, EventArgs e)
        {
            RuleHandler.MatchAndApplyRuleToAllWindows();
        }

        private void OnHookTriggered(object sender, HookTriggeredEventArgs e)
        {
            RuleHandler.MatchAndApplyRuleToWindow(e.WindowHandle);
        }

        private void WindowOnDestroy(object sender, EventArgs e)
        {
            _notifyIcon.Deactivate();
            User32.PostQuitMessage();
        }

        private void OnCommandInvoked(object sender, CommandInvokedEventArgs e)
        {
            switch (e.Id)
            {
                case IDM_EXIT:
                    _window.PostDestroy();
                    break;

                case IDM_RELOAD_CONFIG:
                    RuleHandler.ConfigSource.Reload();
                    RuleHandler.LoadConfig();
                    RuleHandler.MatchAndApplyRuleToAllWindows();
                    break;

                case IDM_REAPPLY:
                    RuleHandler.MatchAndApplyRuleToAllWindows();
                    break;

                case IDM_DEFAULT_THEME_MODE:
                    RuleHandler.GlobalRule.TitlebarColor = TitlebarColorMode.Default;
                    break;

                case IDM_LIGHT_THEME_MODE:
                    RuleHandler.GlobalRule.TitlebarColor = TitlebarColorMode.Light;
                    break;

                case IDM_DARK_THEME_MODE:
                    RuleHandler.GlobalRule.TitlebarColor = TitlebarColorMode.Dark;
                    break;

                case IDM_SET_DEFAULT_BACKDROP:
                    RuleHandler.GlobalRule.BackdropPreference = BackdropType.Default;
                    break;

                case IDM_SET_NO_BACKDROP:
                    RuleHandler.GlobalRule.BackdropPreference = BackdropType.None;
                    break;

                case IDM_SET_MICA_BACKDROP:
                    RuleHandler.GlobalRule.BackdropPreference = BackdropType.Mica;
                    break;

                case IDM_SET_ACRYLIC_BACKDROP:
                    RuleHandler.GlobalRule.BackdropPreference = BackdropType.Acrylic;
                    break;

                case IDM_SET_TABBED_BACKDROP:
                    RuleHandler.GlobalRule.BackdropPreference = BackdropType.Tabbed;
                    break;

                case IDM_ENABLE_EXTEND_FRAME:
                    RuleHandler.GlobalRule.ExtendFrameIntoClientArea = true;
                    break;

                case IDM_DISABLE_EXTEND_FRAME:
                    RuleHandler.GlobalRule.ExtendFrameIntoClientArea = false;
                    break;
            }
        }

        private void OnShowContextMenu(object sender, EventArgs e)
        {
            using var themeModeMenu = new PopupMenu();
            themeModeMenu.AddCheckedTextItem(IDM_DEFAULT_THEME_MODE, "Default",
                RuleHandler.GlobalRule.TitlebarColor == TitlebarColorMode.Default);
            themeModeMenu.AddCheckedTextItem(IDM_DARK_THEME_MODE, "Light",
                RuleHandler.GlobalRule.TitlebarColor == TitlebarColorMode.Light);
            themeModeMenu.AddCheckedTextItem(IDM_DARK_THEME_MODE, "Dark",
                RuleHandler.GlobalRule.TitlebarColor == TitlebarColorMode.Dark);

            using var micaModeMenu = new PopupMenu();
            micaModeMenu.AddCheckedTextItem(IDM_SET_DEFAULT_BACKDROP, "Default",
                RuleHandler.GlobalRule.BackdropPreference == BackdropType.Default);
            micaModeMenu.AddCheckedTextItem(IDM_SET_NO_BACKDROP, "Prefer Disabled",
                RuleHandler.GlobalRule.BackdropPreference == BackdropType.None);
            micaModeMenu.AddCheckedTextItem(IDM_SET_MICA_BACKDROP, "Prefer Mica (Tinted)",
                RuleHandler.GlobalRule.BackdropPreference == BackdropType.Mica);
            #if !DEBUG
            if (SystemBackdrop.IsSupported)
            #endif
            {
                micaModeMenu.AddCheckedTextItem(IDM_SET_ACRYLIC_BACKDROP, "Prefer Acrylic",
                    RuleHandler.GlobalRule.BackdropPreference == BackdropType.Acrylic);
                micaModeMenu.AddCheckedTextItem(IDM_SET_TABBED_BACKDROP, "Prefer Tabbed (Blurred)",
                    RuleHandler.GlobalRule.BackdropPreference == BackdropType.Tabbed);
            }

            using var extendFrameMenu = new PopupMenu();
            extendFrameMenu.AddCheckedTextItem(IDM_ENABLE_EXTEND_FRAME, "Enable", RuleHandler.GlobalRule.ExtendFrameIntoClientArea);
            extendFrameMenu.AddCheckedTextItem(IDM_DISABLE_EXTEND_FRAME, "Disable", !RuleHandler.GlobalRule.ExtendFrameIntoClientArea);

            using var menu = new PopupMenu
            {
                Parent = _window
            };
            menu.AddSubMenu("Titlebar Color Mode", themeModeMenu);
            menu.AddSubMenu("Backdrop Type Preference", micaModeMenu);
            menu.AddSubMenu("Extend Frame Into Client Area", extendFrameMenu);
            menu.AddSeparatorItem();
            menu.AddTextItem(IDM_RELOAD_CONFIG, "Reload config file");
            menu.AddTextItem(IDM_REAPPLY, "Reapply rules");
            menu.AddTextItem(IDM_EXIT, "Exit");

            menu.Show();
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            _components.Dispose();
        }
    }
}
