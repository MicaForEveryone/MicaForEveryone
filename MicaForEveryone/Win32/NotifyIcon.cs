using System;
using System.ComponentModel;
using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public class NotifyIcon : Component
    {
        private Shell32.NOTIFYICONDATA _notifyIconData;

        public NotifyIcon()
        {
            InitializeComponent();
        }

        public NotifyIcon(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public MainWindow Parent { get; set; }

        public uint Id { get; set; } = 0;

        public uint CallbackId { get; set; }

        public HICON Icon { get; set; }

        public string Title { get; set; }

        public void Activate()
        {
            _notifyIconData.uFlags = Shell32.NIF.NIF_ICON | Shell32.NIF.NIF_TIP | Shell32.NIF.NIF_MESSAGE;
            _notifyIconData.hwnd = Parent.Handle;
            _notifyIconData.uID = Id;
            _notifyIconData.uCallbackMessage = CallbackId;
            _notifyIconData.hIcon = Icon;
            _notifyIconData.szTip = Title;
            if (!Shell32.Shell_NotifyIcon(Shell32.NIM.NIM_ADD, _notifyIconData))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            _notifyIconData.uTimeoutOrVersion = 4;
            if (!Shell32.Shell_NotifyIcon(Shell32.NIM.NIM_SETVERSION, _notifyIconData))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            Parent.AddCustomHandler((User32.WindowMessage)CallbackId, CallbackHandler);
        }

        public void Deactivate()
        {
            Parent.RemoveCustomHandler((User32.WindowMessage)CallbackId);

            Shell32.Shell_NotifyIcon(Shell32.NIM.NIM_DELETE, _notifyIconData);
        }

        public RECT GetRect()
        {
            var id = new Shell32.NOTIFYICONIDENTIFIER(Parent.Handle, Id);
            Shell32.Shell_NotifyIconGetRect(id, out var result).ThrowIfFailed();
            return result;
        }

        private void InitializeComponent()
        {
            Disposed += OnDisposed;
        }

        private IntPtr CallbackHandler(HWND hwnd, uint umsg, IntPtr wParam, IntPtr lParam)
        {
            switch (Macros.LOWORD(lParam))
            {
                case (ushort)User32.WindowMessage.WM_CONTEXTMENU:
                    OnShowContextMenu();
                    break;
            }

            return User32.DefWindowProc(hwnd, umsg, wParam, lParam);
        }

        private void OnShowContextMenu()
        {
            ShowContextMenu?.Invoke(this, EventArgs.Empty);
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            Deactivate();
        }

        public event EventHandler ShowContextMenu;
    }
}
