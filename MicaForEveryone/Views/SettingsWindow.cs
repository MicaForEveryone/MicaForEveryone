using System;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.UI;
using MicaForEveryone.UI.Brushes;
using MicaForEveryone.UI.ViewModels;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.Views
{
    internal class SettingsWindow : XamlWindow
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct WTA_OPTIONS
        {
            public uint Flags;
            public uint Mask;
        }

        [DllImport("uxtheme.dll", PreserveSig = false)]
        private static extern void SetWindowThemeAttribute([In] IntPtr hwnd, [In] uint eAttribute, [In] ref WTA_OPTIONS pvAttribute, [In] int cbAttribute);

        private readonly XamlMicaBrush _backgroundBrush;

        public SettingsWindow() : this(new())
        {
        }

        private SettingsWindow(SettingsView view) : base(view)
        {
            Style = WindowStyles.WS_OVERLAPPEDWINDOW & ~WindowStyles.WS_MAXIMIZEBOX;
            Width = 820;
            Height = 560;

            _backgroundBrush = new XamlMicaBrush(View, this);

            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("SettingsTitle/Text");

            view.ViewModel = ViewModel;
            view.ActualThemeChanged += View_ActualThemeChanged;
            view.Loaded += View_Loaded;
        }

        private ISettingsViewModel ViewModel { get; } =
            Program.CurrentApp.Container.GetService<ISettingsViewModel>();

        public override void Activate()
        {
            base.Activate();

            CenterToWindowScaled(GetDesktopWindow());
            UpdatePosition();

            ((Grid)((SettingsView)View).Content).Background = _backgroundBrush;

            WTA_OPTIONS options = new WTA_OPTIONS();
            options.Flags = 1 | 2 | 4 | 8;
            options.Mask = 1 | 2 | 4 | 8;
            SetWindowThemeAttribute(Handle, 1, ref options, Marshal.SizeOf<WTA_OPTIONS>());

            ShowWindow();

            DesktopWindowManager.SetImmersiveDarkMode(Handle, Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode == TitlebarColorMode.Dark);
            DesktopWindowManager.EnableMicaIfSupported(Handle);
            SetForegroundWindow();
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Initialize(sender);
        }

        private void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            DesktopWindowManager.SetImmersiveDarkMode(Handle, Program.CurrentApp.Container.GetService<IViewService>().SystemColorMode == TitlebarColorMode.Dark);
        }
    }
}
