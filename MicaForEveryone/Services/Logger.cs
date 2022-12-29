using System;
using System.Collections.Generic;
using System.Text;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Models;

namespace MicaForEveryone.Services
{
	internal class Logger : ILogger
	{
		private List<LogEntry> _logs = new();
		
		public void Add(LogEntry entry)
		{
			_logs.Add(entry);
			LogEntryAdded?.Invoke(entry);
		}

		public IEnumerable<LogEntry> GetLogEntries()
		{
			return _logs;
		}
		
		public event Action<LogEntry> LogEntryAdded;
	}
}
