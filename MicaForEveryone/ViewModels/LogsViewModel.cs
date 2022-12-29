using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.ViewModels
{
	internal class LogsViewModel : ObservableObject, ILogsViewModel
	{
		private ILogger _logger;
		
		public LogsViewModel(ILogger logger)
		{
			_logger = logger;
			Logs = new ObservableCollection<LogEntry>(_logger.GetLogEntries());
			_logger.LogEntryAdded += Logger_LogEntryAdded;
		}

		private void Logger_LogEntryAdded(LogEntry obj)
		{
			Logs.Add(obj);
		}

		public ObservableCollection<LogEntry> Logs { get; }
	}
}
