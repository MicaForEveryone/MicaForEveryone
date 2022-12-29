using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Microsoft.Extensions.DependencyInjection;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Xaml;
using MicaForEveryone.UI;

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
			Style = Win32.PInvoke.WindowStyles.WS_OVERLAPPEDWINDOW | Win32.PInvoke.WindowStyles.WS_VISIBLE;
			Width = 800;
			Height = 450;
			_view = view;
			_view.ViewModel = ViewModel;
		}

		public ILogsViewModel ViewModel { get; } = Program.CurrentApp.Container.GetRequiredService<ILogsViewModel>();
	}
}
