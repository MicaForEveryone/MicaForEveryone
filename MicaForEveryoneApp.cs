using System;
using System.ComponentModel;
using Vanara.PInvoke;
using IntPtr = System.IntPtr;

namespace MicaForEveryone
{
    internal class MicaForEveryoneApp : Component
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

        private readonly IContainer _components = new Container();
        private readonly WinEventHook _eventHook = new();

        private Kernel32.SafeHINSTANCE _instanceHandle;
        private User32.SafeHICON _appIconHandle;
        private User32.SafeHWND _windowHandle;

        private Shell32.NOTIFYICONDATA _notifyIconData;

        private readonly RuleHandler _ruleHandler = new();

        public MicaForEveryoneApp()
        {
            InitializeComponents();
        }

        public void Run()
        {
            // show notify icon
            _notifyIconData.uFlags = Shell32.NIF.NIF_ICON | Shell32.NIF.NIF_TIP | Shell32.NIF.NIF_MESSAGE;
            _notifyIconData.hwnd = _windowHandle;
            _notifyIconData.uID = 0;
            _notifyIconData.uCallbackMessage = WM_NOTIFYICON;
            _notifyIconData.hIcon = _appIconHandle;
            _notifyIconData.szTip = "Mica For Everyone";
            Shell32.Shell_NotifyIcon(Shell32.NIM.NIM_ADD, _notifyIconData);
            _notifyIconData.uTimeoutOrVersion = 4;
            Shell32.Shell_NotifyIcon(Shell32.NIM.NIM_SETVERSION, _notifyIconData);

            User32.ShowWindow(_windowHandle, ShowWindowCommand.SW_SHOW);

            _ruleHandler.ApplyToAllWindows();

            _eventHook.Hook(User32.EventConstants.EVENT_OBJECT_CREATE, User32.EventConstants.EVENT_OBJECT_CREATE);

            // main loop
            while (User32.GetMessage(out var msg, HWND.NULL, 0, 0))
            {
                User32.TranslateMessage(msg);
                User32.DispatchMessage(msg);
            }
        }

        private void InitializeComponents()
        {
            _instanceHandle = Kernel32.GetModuleHandle();
            _appIconHandle = User32.LoadIcon(HINSTANCE.NULL, User32.IDI_APPLICATION);

            var windowClass = new User32.WNDCLASS
            {
                hInstance = _instanceHandle,
                lpszClassName = "Mica For Everyone",
                lpfnWndProc = WndProc,
            };
            User32.RegisterClass(windowClass);

            _windowHandle = User32.CreateWindowEx(
                0,
                windowClass.lpszClassName,
                "Mica For Everyone",
                0,
                0, 0, 0, 0,
                HWND.HWND_MESSAGE,
                HMENU.NULL, 
                _instanceHandle);

            _eventHook.HookTriggered += OnHookTriggered;
            _components.Add(_eventHook);

            Disposed += OnDisposed;
        }

        private IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wparam, IntPtr lparam)
        {
            switch (umsg)
            {
                case WM_NOTIFYICON:
                    if (Macros.SignedLOWORD(lparam) == (int)User32.WindowMessage.WM_CONTEXTMENU)
                    {
                        ShowContextMenu(hwnd);
                        return IntPtr.Zero;
                    }
                    break;
                case (uint)User32.WindowMessage.WM_COMMAND:
                    switch (Macros.LOWORD(wparam))
                    {
                        case (ushort)IDM_EXIT:
                            User32.SendMessage(_windowHandle, User32.WindowMessage.WM_DESTROY);
                            return IntPtr.Zero;

                        case (ushort)IDM_REAPPLY:
                            _ruleHandler.ApplyToAllWindows();
                            return IntPtr.Zero;

                        case (ushort)IDM_DEFAULT_THEME_MODE:
                            _ruleHandler.GlobalRule.Theme = ThemeMode.Default;
                            return IntPtr.Zero;

                        case (ushort)IDM_LIGHT_THEME_MODE:
                            _ruleHandler.GlobalRule.Theme = ThemeMode.ForceLight;
                            return IntPtr.Zero;

                        case (ushort)IDM_DARK_THEME_MODE:
                            _ruleHandler.GlobalRule.Theme = ThemeMode.ForceDark;
                            return IntPtr.Zero;

                        case (ushort)IDM_DEFAULT_MICA_MODE:
                            _ruleHandler.GlobalRule.Mica = MicaMode.Default;
                            return IntPtr.Zero;

                        case (ushort)IDM_FORCE_MICA_MODE:
                            _ruleHandler.GlobalRule.Mica = MicaMode.ForceMica;
                            return IntPtr.Zero;

                        case (ushort)IDM_FORCE_NO_MICA_MODE:
                            _ruleHandler.GlobalRule.Mica = MicaMode.ForceNoMica;
                            return IntPtr.Zero;
                    }
                    break;
                case (uint)User32.WindowMessage.WM_DESTROY:
                    Dispose();
                    return IntPtr.Zero;
            }
            return User32.DefWindowProc(hwnd, umsg, wparam, lparam);
        }

        private void OnHookTriggered(object sender, HookTriggeredEventArgs e)
        {
            _ruleHandler.ApplyToWindow(e.WindowHandle);
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            // hide notify icon
            Shell32.Shell_NotifyIcon(Shell32.NIM.NIM_DELETE, _notifyIconData);

            User32.DestroyWindow(_windowHandle);

            _components.Dispose();
            _windowHandle.Dispose();
            _instanceHandle.Dispose();

            User32.PostQuitMessage();
        }

        private void ShowContextMenu(HWND windowHandle)
        {
            using var themeModeMenu = new PopupMenu();
            themeModeMenu.AddCheckedTextItem(IDM_DEFAULT_THEME_MODE, "Default", _ruleHandler.GlobalRule.Theme == ThemeMode.Default);
            themeModeMenu.AddCheckedTextItem(IDM_DARK_THEME_MODE, "Force Light", _ruleHandler.GlobalRule.Theme == ThemeMode.ForceLight);
            themeModeMenu.AddCheckedTextItem(IDM_DARK_THEME_MODE, "Force Dark", _ruleHandler.GlobalRule.Theme == ThemeMode.ForceDark);

            using var micaModeMenu = new PopupMenu();
            micaModeMenu.AddCheckedTextItem(IDM_DEFAULT_MICA_MODE, "Default", _ruleHandler.GlobalRule.Mica == MicaMode.Default);
            micaModeMenu.AddCheckedTextItem(IDM_FORCE_MICA_MODE, "Force Mica", _ruleHandler.GlobalRule.Mica == MicaMode.ForceMica);
            micaModeMenu.AddCheckedTextItem(IDM_FORCE_NO_MICA_MODE, "Force Disable Mica", _ruleHandler.GlobalRule.Mica == MicaMode.ForceNoMica);

            using var menu = new PopupMenu();
            menu.AddSubMenu("Theme Mode", themeModeMenu);
            menu.AddSubMenu("Mica Mode", micaModeMenu);
            menu.AddSeparatorItem();
            menu.AddTextItem(IDM_REAPPLY, "Reapply rules");
            menu.AddTextItem(IDM_EXIT, "Exit");

            menu.Show(windowHandle);
        }
    }
}
