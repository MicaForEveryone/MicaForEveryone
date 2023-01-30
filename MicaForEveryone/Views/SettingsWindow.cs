using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Core.Ui.Views;
using MicaForEveryone.Core.Ui.ViewModels;
using MicaForEveryone.UI;
using MicaForEveryone.UI.Brushes;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;
using MicaForEveryone.Xaml;

namespace MicaForEveryone.Views
{
    internal class SettingsWindow : XamlWindow, ISettingsView
    {
        private readonly XamlMicaBrush _backgroundBrush;

        public SettingsWindow() : this(new())
        {
        }

        private SettingsWindow(SettingsView view) : base(view)
        {
            Style = WindowStyles.WS_OVERLAPPEDWINDOW & ~WindowStyles.WS_MAXIMIZEBOX;
            Width = 1000;
            Height = 700;

            _backgroundBrush = new XamlMicaBrush(View, this);

            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("SettingsTitle/Text");

            view.ViewModel = ViewModel;
            view.ActualThemeChanged += View_ActualThemeChanged;
        }

        private ISettingsViewModel ViewModel { get; } =
            Program.Container.GetService<ISettingsViewModel>();

        public override void Activate()
        {
            base.Activate();

            CenterToWindowScaled(GetDesktopWindow());
            UpdatePosition();

            ((Grid)((SettingsView)View).Content).Background = _backgroundBrush;

            EnableWindowThemeAttribute(WTNCA.WTNCA_NODRAWCAPTION | WTNCA.WTNCA_NODRAWICON | WTNCA.WTNCA_NOSYSMENU);

            DesktopWindowManager.SetImmersiveDarkMode(Handle, View.ActualTheme == ElementTheme.Dark);
            DesktopWindowManager.EnableMicaIfSupported(Handle);

            ViewModel.Attach(this);
            ShowWindow();
            SetForegroundWindow();
        }

        private void View_ActualThemeChanged(FrameworkElement sender, object args)
        {
            DesktopWindowManager.SetImmersiveDarkMode(Handle, sender.ActualTheme == ElementTheme.Dark);
        }
    }
}
