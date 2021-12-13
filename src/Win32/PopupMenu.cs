using System;
using System.Runtime.InteropServices;
using Vanara.InteropServices;
using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public class PopupMenu : IDisposable
    {
        private readonly User32.SafeHMENU _menuHandle;

        public PopupMenu()
        {
            _menuHandle = User32.CreatePopupMenu();
            if (_menuHandle.IsNull)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public IWindow Parent { get; set; }

        public void Dispose()
        {
            User32.DestroyMenu(_menuHandle);
            _menuHandle.Dispose();
        }

        private void AddMenuItem(ref User32.MENUITEMINFO item)
        {
            var pos = (uint)User32.GetMenuItemCount(_menuHandle) + 1;
            if (User32.InsertMenuItem(_menuHandle, pos, true, ref item))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }

        public void AddTextItem(uint id, string text)
        {
            var itemInfo = new User32.MENUITEMINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(User32.MENUITEMINFO)),
                fMask = User32.MenuItemInfoMask.MIIM_FTYPE | User32.MenuItemInfoMask.MIIM_ID | User32.MenuItemInfoMask.MIIM_STRING,
                fType = User32.MenuItemType.MFT_STRING,
                dwTypeData = new StrPtrAuto(text),
                cch = (uint)text.Length + 1,
                wID = id,
            };

            AddMenuItem(ref itemInfo);
        }

        public void AddCheckedTextItem(uint id, string text, bool isChecked)
        {
            var itemInfo = new User32.MENUITEMINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(User32.MENUITEMINFO)),
                fMask = User32.MenuItemInfoMask.MIIM_FTYPE | User32.MenuItemInfoMask.MIIM_ID | User32.MenuItemInfoMask.MIIM_STRING | User32.MenuItemInfoMask.MIIM_STATE,
                fType = User32.MenuItemType.MFT_STRING,
                dwTypeData = new StrPtrAuto(text),
                cch = (uint)text.Length + 1,
                wID = id,
                fState = isChecked ? User32.MenuItemState.MFS_CHECKED : User32.MenuItemState.MFS_UNCHECKED,
            };

            AddMenuItem(ref itemInfo);
        }

        public void AddSubMenu(string text, PopupMenu menu)
        {
            var itemInfo = new User32.MENUITEMINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(User32.MENUITEMINFO)),
                fMask = User32.MenuItemInfoMask.MIIM_FTYPE | User32.MenuItemInfoMask.MIIM_STRING | User32.MenuItemInfoMask.MIIM_SUBMENU,
                fType = User32.MenuItemType.MFT_STRING,
                dwTypeData = new StrPtrAuto(text),
                cch = (uint)text.Length + 1,
                hSubMenu = menu._menuHandle,
            };

            AddMenuItem(ref itemInfo);
        }

        public void AddSeparatorItem()
        {
            var itemInfo = new User32.MENUITEMINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(User32.MENUITEMINFO)),
                fMask = User32.MenuItemInfoMask.MIIM_TYPE,
                fType = User32.MenuItemType.MFT_SEPARATOR,
            };

            AddMenuItem(ref itemInfo);
        }

        public void Show()
        {
            User32.SetForegroundWindow(Parent.Handle);

            var uFlags = User32.TrackPopupMenuFlags.TPM_RIGHTBUTTON;
            if (User32.GetSystemMetrics(User32.SystemMetric.SM_MENUDROPALIGNMENT) != 0)
            {
                uFlags |= User32.TrackPopupMenuFlags.TPM_RIGHTALIGN;
            }
            else
            {
                uFlags |= User32.TrackPopupMenuFlags.TPM_LEFTALIGN;
            }

            if (!User32.GetCursorPos(out var point))
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }

            if (User32.TrackPopupMenuEx(_menuHandle, uFlags, point.X, point.Y, Parent.Handle) == 0)
            {
                Kernel32.GetLastError().ThrowIfFailed();
            }
        }
    }
}
