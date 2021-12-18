using System;
using System.ComponentModel;
using Windows.UI.Xaml.Hosting;
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

        private readonly IContainer _components = new Container();

        private WinEventHook _eventHook;
        private WindowsXamlManager _xamlManager;
        private MainWindow _window;
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

            _xamlManager = WindowsXamlManager.InitializeForCurrentThread();

            // initialize main window
            _window = new MainWindow(_components)
            {
                Title = "Mica For Windows",
                Icon = User32.LoadIcon(HINSTANCE.NULL, User32.IDI_APPLICATION),
            };
            _window.Create += WindowOnCreate;
            _window.Destroy += WindowOnDestroy;

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
        
        private void OnShowContextMenu(object sender, EventArgs e)
        {
            _window.ShowContextFlyout(_notifyIcon.GetRect());
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            _xamlManager.Dispose();
            _components.Dispose();
        }
    }
}
