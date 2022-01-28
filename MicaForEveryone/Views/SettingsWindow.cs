using Microsoft.Extensions.DependencyInjection;
using MicaForEveryone.UI;
using MicaForEveryone.Xaml;
using MicaForEveryone.Win32;
using MicaForEveryone.Models;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;
using MicaForEveryone.Interfaces;

namespace MicaForEveryone.Views
{
    internal class SettingsWindow : XamlWindow
    {
        public SettingsWindow() : this(new())
        {
        }

        private SettingsWindow(SettingsView view) : base(view)
        {
            ClassName = nameof(SettingsWindow);
            Title = "Settings";
            Icon = LoadIcon(HINSTANCE.NULL, IDI_APPLICATION);
            Style = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE;
            Width = 400;
            Height = 500;
            Create += SettingsWindow_Create;
        }

        private void SettingsWindow_Create(object sender, Win32EventArgs e)
        {
            e.WindowHandle.ApplyBackdropRule(BackdropType.Mica);
            e.WindowHandle.ApplyTitlebarColorRule(
                Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode,
                TitlebarColorMode.Default);
        }
    }
}
