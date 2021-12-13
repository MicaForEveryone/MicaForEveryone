using System;
using System.ComponentModel;
using Vanara.PInvoke;

using MicaForEveryone.Win32;
using MicaForEveryone.Rules;

namespace MicaForEveryone
{
    internal class Application : Component
    {
        private const uint WM_NOTIFYICON = User32.WM_APP + 1;

        private const uint IDM_EXIT = 0;
        private const uint IDM_REAPPLY = 1;

        private const uint IDM_DEFAULT_THEME_MODE = 10;
        private const uint IDM_LIGHT_THEME_MODE = 11;
        private const uint IDM_DARK_THEME_MODE = 12;

        private const uint IDM_DEFAULT_MICA_MODE = 20;
        private const uint IDM_FORCE_MICA_MODE = 21;
        private const uint IDM_FORCE_NO_MICA_MODE = 22;

        private const uint IDM_ENABLE_EXTEND_FRAME = 30;
        private const uint IDM_DISABLE_EXTEND_FRAME = 31;

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
                case (ushort)IDM_EXIT:
                    _window.PostDestroy();
                    break;

                case (ushort)IDM_REAPPLY:
                    RuleHandler.ConfigSource.Reload();
                    RuleHandler.MatchAndApplyRuleToAllWindows();
                    break;

                case (ushort)IDM_DEFAULT_THEME_MODE:
                    RuleHandler.GlobalRule.TitlebarColor = TitlebarColorMode.Default;
                    break;

                case (ushort)IDM_LIGHT_THEME_MODE:
                    RuleHandler.GlobalRule.TitlebarColor = TitlebarColorMode.Light;
                    break;

                case (ushort)IDM_DARK_THEME_MODE:
                    RuleHandler.GlobalRule.TitlebarColor = TitlebarColorMode.Dark;
                    break;

                case (ushort)IDM_DEFAULT_MICA_MODE:
                    RuleHandler.GlobalRule.MicaPreference = MicaPreference.Default;
                    break;

                case (ushort)IDM_FORCE_MICA_MODE:
                    RuleHandler.GlobalRule.MicaPreference = MicaPreference.PreferEnabled;
                    break;

                case (ushort)IDM_FORCE_NO_MICA_MODE:
                    RuleHandler.GlobalRule.MicaPreference = MicaPreference.PreferDisabled;
                    break;

                case (ushort)IDM_ENABLE_EXTEND_FRAME:
                    RuleHandler.GlobalRule.ExtendFrameIntoClientArea = true;
                    break;
                    
                case (ushort)IDM_DISABLE_EXTEND_FRAME:
                    RuleHandler.GlobalRule.ExtendFrameIntoClientArea = false;
                    break;
            }
        }

        private void OnShowContextMenu(object sender, EventArgs e)
        {
            using var themeModeMenu = new PopupMenu();
            themeModeMenu.AddCheckedTextItem(IDM_DEFAULT_THEME_MODE, "Default", RuleHandler.GlobalRule.TitlebarColor == TitlebarColorMode.Default);
            themeModeMenu.AddCheckedTextItem(IDM_DARK_THEME_MODE, "Light", RuleHandler.GlobalRule.TitlebarColor == TitlebarColorMode.Light);
            themeModeMenu.AddCheckedTextItem(IDM_DARK_THEME_MODE, "Dark", RuleHandler.GlobalRule.TitlebarColor == TitlebarColorMode.Dark);

            using var micaModeMenu = new PopupMenu();
            micaModeMenu.AddCheckedTextItem(IDM_DEFAULT_MICA_MODE, "Default", RuleHandler.GlobalRule.MicaPreference == MicaPreference.Default);
            micaModeMenu.AddCheckedTextItem(IDM_FORCE_MICA_MODE, "Prefer Enabled", RuleHandler.GlobalRule.MicaPreference == MicaPreference.PreferEnabled);
            micaModeMenu.AddCheckedTextItem(IDM_FORCE_NO_MICA_MODE, "Prefer Disabled", RuleHandler.GlobalRule.MicaPreference == MicaPreference.PreferDisabled);

            using var extendFrameMenu = new PopupMenu();
            extendFrameMenu.AddCheckedTextItem(IDM_ENABLE_EXTEND_FRAME, "Enable", RuleHandler.GlobalRule.ExtendFrameIntoClientArea);
            extendFrameMenu.AddCheckedTextItem(IDM_DISABLE_EXTEND_FRAME, "Disable", !RuleHandler.GlobalRule.ExtendFrameIntoClientArea);

            using var menu = new PopupMenu
            {
                Parent = _window
            };
            menu.AddSubMenu("Titlebar Color Mode", themeModeMenu);
            menu.AddSubMenu("Mica Preference", micaModeMenu);
            menu.AddSubMenu("Extend Frame Into Client Area", extendFrameMenu);
            menu.AddSeparatorItem();
            menu.AddTextItem(IDM_REAPPLY, "Reapply rules");
            menu.AddTextItem(IDM_EXIT, "Exit");

            menu.Show();
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            _components.Dispose();
        }
    }
}
