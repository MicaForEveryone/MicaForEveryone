using System;
using System.Diagnostics;
using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.Models
{
    public class TargetWindow
    {
        //public static TargetWindow FromAutomationElement(IUIAutomationElement element)
        //{
        //    return new TargetWindow
        //    {
        //        WindowHandle = element.CurrentNativeWindowHandle,
        //        Title = element.CurrentName,
        //        ClassName = element.CurrentClassName,
        //        ProcessName = Process.GetProcessById(element.CurrentProcessId).ProcessName,
        //    };
        //}

        public static TargetWindow FromWindow(Window window)
        {
            return new TargetWindow
            {
                WindowHandle = window.Handle,
                Title = window.GetText(),
                ClassName = window.Class.Name,
                ProcessName = Process.GetProcessById((int)window.GetProcessId()).ProcessName,
            };
        }

        private TargetWindow() { }

        public IntPtr WindowHandle { get; private set; }
        public string Title { get; private set; }
        public string ClassName { get; private set; }
        public string ProcessName { get; private set; }

        public void ApplyBackdropRule(BackdropType type)
        {
            if (type == BackdropType.Default)
                    return;

            if (DesktopWindowManager.IsBackdropTypeSupported)
            {
                DesktopWindowManager.SetBackdropType(WindowHandle, type);
            }
            else if (DesktopWindowManager.IsUndocumentedMicaSupported && type < BackdropType.Acrylic)
            {
                DesktopWindowManager.SetMica(WindowHandle, type == BackdropType.Mica);
            }
        }

        public void ApplyTitlebarColorRule(TitlebarColorMode targetMode, TitlebarColorMode systemMode)
        {
            if (!DesktopWindowManager.IsImmersiveDarkModeSupported) return;
            switch (targetMode)
            {
                case TitlebarColorMode.Default:
                    return;
                case TitlebarColorMode.System:
                    if (systemMode == TitlebarColorMode.System) return;
                    ApplyTitlebarColorRule(systemMode, TitlebarColorMode.Default);
                    return;
                case TitlebarColorMode.Light:
                    DesktopWindowManager.SetImmersiveDarkMode(WindowHandle, false);
                    return;
                case TitlebarColorMode.Dark:
                    DesktopWindowManager.SetImmersiveDarkMode(WindowHandle, true);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ApplyRule(IRule rule, TitlebarColorMode systemTitlebarColorMode)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Applying rule `{rule}` to `{Title}` ({ClassName}, {ProcessName})");
#endif
            ApplyTitlebarColorRule(rule.TitleBarColor, systemTitlebarColorMode);
            ApplyBackdropRule(rule.BackdropPreference);

            if (rule.ExtendFrameIntoClientArea)
                DesktopWindowManager.ExtendFrameIntoClientArea(WindowHandle);

            if (rule.EnableBlurBehind)
                DesktopWindowManager.EnableBlurBehind(WindowHandle);
        }
    }
}
