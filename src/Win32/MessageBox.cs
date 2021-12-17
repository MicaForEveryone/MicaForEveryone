using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanara.PInvoke;

namespace MicaForEveryone.Win32
{
    public static class MessageBox
    {
        public static void ShowErrorMessage(string message)
        {
            User32.MessageBox(HWND.NULL, message, "Error",
                User32.MB_FLAGS.MB_OK | User32.MB_FLAGS.MB_ICONERROR);
        }

        public static void ShowErrorTaskDialog(string mainInstruction, string message)
        {
            var config = new ComCtl32.TASKDIALOGCONFIG
            {
                WindowTitle = "Mica For Everyone",
                MainInstruction = mainInstruction,
                Content = message,
                mainIcon = (IntPtr) ComCtl32.TaskDialogIcon.TD_ERROR_ICON,
            };
            ComCtl32.TaskDialogIndirect(config, out _, out _, out _);
        }
    }
}
