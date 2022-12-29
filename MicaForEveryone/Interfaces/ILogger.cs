using MicaForEveryone.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicaForEveryone.Interfaces
{
	internal interface ILogger
	{
		void Add(LogEntry entry);

		IEnumerable<LogEntry> GetLogEntries();

		event Action<LogEntry> LogEntryAdded;
	}
}
