using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Xaml;
using MicaForEveryone.UI;
using MicaForEveryone.Win32;
using MicaForEveryone.Win32.PInvoke;

namespace MicaForEveryone.Views
{
	internal class LogsWindow : XamlWindow
	{
		private LogsView _view;

		public LogsWindow() : this(new())
		{
		}

		private LogsWindow(LogsView view) : base(view)
		{
			Style = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE;
			Width = 800;
			Height = 450;
			_view = view;
			_view.ViewModel = ViewModel;
            _view.Loaded += View_Loaded;

            var resources = ResourceLoader.GetForCurrentView();
            Title = resources.GetString("LogsWindow/Title");
		}

        public ILogsViewModel ViewModel { get; } = Program.CurrentApp.Container.GetRequiredService<ILogsViewModel>();

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            var bgColor = ((SolidColorBrush)_view.Background)?.Color;
            var fgColor = ((SolidColorBrush)_view.Foreground)?.Color;
            if (bgColor.HasValue == false || fgColor.HasValue == false) return;
			DesktopWindowManager.SetCaptionColor(Handle, new COLORREF(bgColor.Value.R, bgColor.Value.G, bgColor.Value.B));
			DesktopWindowManager.SetCaptionTextColor(Handle, new COLORREF(fgColor.Value.R, fgColor.Value.G, fgColor.Value.B));
        }
	}
}
