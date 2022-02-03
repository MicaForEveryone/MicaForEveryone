using System;
using System.Diagnostics;
using System.Windows.Automation;
using Vanara.PInvoke;

namespace MicaForEveryone.Models
{
    public class TargetWindow
    {
        public static TargetWindow FromAutomationElement(AutomationElement element)
        {
            return new TargetWindow
            {
                WindowHandle = new HWND(new IntPtr(element.Current.NativeWindowHandle)),
                Title = element.Current.Name,
                ClassName = element.Current.ClassName,
                ProcessName = Process.GetProcessById(element.Current.ProcessId).ProcessName,
            };
        }

        private TargetWindow() { }

        public HWND WindowHandle { get; private set; }
        public string Title { get; private set; }
        public string ClassName { get; private set; }
        public string ProcessName { get; private set; }
    }
}
