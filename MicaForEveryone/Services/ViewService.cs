using Windows.UI.Xaml;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;
using MicaForEveryone.Views;

namespace MicaForEveryone.Services
{
    public class ViewService : IViewService
    {
        public ViewService(IViewModel viewModel)
        {
            MainWindow.View.ActualThemeChanged += MainWindow_ThemeChanged;

            viewModel.Attach(MainWindow);
        }

        public MainWindow MainWindow { get; } = new();

        private void MainWindow_ThemeChanged(FrameworkElement sender, object args)
        {
            SystemColorMode = Application.Current.RequestedTheme switch
            {
                ApplicationTheme.Light => TitlebarColorMode.Light,
                ApplicationTheme.Dark => TitlebarColorMode.Dark,
                _ => TitlebarColorMode.Default,
            };
            MainWindow.RequestRematchRules();
        }

        public TitlebarColorMode SystemColorMode { get; private set; }
    }
}
