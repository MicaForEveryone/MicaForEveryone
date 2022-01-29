using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI;
using MicaForEveryone.UI.Brushes;
using MicaForEveryone.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

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
            Style = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE;
            Width = 400;
            Height = 500;
            Create += SettingsWindow_Create;

            view.ViewModel = ViewModel;
            view.ActualThemeChanged += View_ActualThemeChanged;

            var micaBrush = new XamlMicaBrush();
            micaBrush.RootElement = view;
            micaBrush.FocusableWindow = this;
            if (view.Content is Windows.UI.Xaml.Controls.Grid element)
            {
                element.Background = micaBrush;
            }
        }

        private ISettingsViewModel ViewModel { get; } =
            Program.CurrentApp.Container.GetService<ISettingsViewModel>();

        private void SettingsWindow_Create(object sender, Win32EventArgs e)
        {
            e.WindowHandle.ApplyBackdropRule(BackdropType.Mica);
            e.WindowHandle.ApplyTitlebarColorRule(
                Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode,
                TitlebarColorMode.Default);
        }

        private void View_ActualThemeChanged(Windows.UI.Xaml.FrameworkElement sender, object args)
        {
            Handle.ApplyTitlebarColorRule(
                Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode,
                TitlebarColorMode.Default);
        }
    }
}
