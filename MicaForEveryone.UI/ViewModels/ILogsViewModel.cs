using MicaForEveryone.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MicaForEveryone.UI.ViewModels
{
	public interface ILogsViewModel
	{
		ObservableCollection<LogEntry> Logs { get; }
	}
}
