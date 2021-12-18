using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public class MainWindow : Component, IWindow
    {
        private User32.SafeHWND _windowHandle;
        private Kernel32.SafeHINSTANCE _instanceHandle;
        private User32.WNDCLASS _class;

        private readonly Dictionary<User32.WindowMessage, User32.WindowProc> _customHandlers = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        
        public string Title { get; set; }

        public HICON Icon { get; set; }

        public HWND Handle => _windowHandle;
        
        public DesktopWindowXamlSource XamlSource { get; private set; }

        public void Activate()
        {
            _class = new User32.WNDCLASS
            {
                hInstance = _instanceHandle,
                lpszClassName = nameof(MainWindow),
                hIcon = Icon,
                lpfnWndProc = WndProc,
            };
            if (User32.RegisterClass(_class) == 0)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            _windowHandle = User32.CreateWindowEx(
                0,
                _class.lpszClassName,
                Title,
                0,
                0, 0, 0, 0,
                HWND.HWND_MESSAGE,
                HMENU.NULL, 
                _instanceHandle);
            if (_windowHandle.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            XamlSource = new DesktopWindowXamlSource();
        }

        public void AddCustomHandler(User32.WindowMessage message, User32.WindowProc handler)
        {
            _customHandlers.Add(message, handler);
        }

        public void RemoveCustomHandler(User32.WindowMessage message)
        {
            _customHandlers.Remove(message);
        }

        public void PostDestroy()
        {
            User32.PostMessage(_windowHandle, (uint) User32.WindowMessage.WM_DESTROY);
        }

        public void ShowContextFlyout(RECT position)
        {
            if (XamlSource.Content.ContextFlyout is MenuFlyout menu)
            {
                menu.ShowAt(XamlSource.Content, new Point(position.X, position.Y));
            }
        }

        private void InitializeComponent()
        {
            Disposed += OnDisposed;

            _instanceHandle = Kernel32.GetModuleHandle();

            if (_instanceHandle.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        private IntPtr WndProc(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            if (_customHandlers.ContainsKey((User32.WindowMessage) umsg))
            {
                return _customHandlers[(User32.WindowMessage) umsg](hwnd, umsg, wParam, lParam);
            }

            switch (umsg)
            {
                case (uint)User32.WindowMessage.WM_CREATE:
                    OnCreate();
                    break;

                case (uint)User32.WindowMessage.WM_COMMAND:
                    OnCommand(Macros.LOWORD(wParam));
                    break;

                case (uint)User32.WindowMessage.WM_DESTROY:
                    OnDestroy();
                    break;
            }
            return User32.DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        private void OnCommand(ushort commandId)
        {
            var args = new CommandInvokedEventArgs(commandId);
            CommandInvoked?.Invoke(this, args);
        }

        private void OnCreate()
        {
            Create?.Invoke(this, EventArgs.Empty);
        }

        private void OnDestroy()
        {
            Destroy?.Invoke(this, EventArgs.Empty);
        }

        private void OnContextFlyout()
        {

        }

        private void OnDisposed(object sender, EventArgs e)
        {
            User32.DestroyWindow(_windowHandle);
            _windowHandle.Dispose();
            _instanceHandle.Dispose();
        }

        public event EventHandler<CommandInvokedEventArgs> CommandInvoked;
        public event EventHandler Create;
        public event EventHandler Destroy;
        public event EventHandler ContextFlyout;
    }

    public class CommandInvokedEventArgs : EventArgs
    {
        public CommandInvokedEventArgs(ushort id)
        {
            Id = id;
        }

        public ushort Id { get; }
    }
}
