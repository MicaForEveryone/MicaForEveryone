using System;
using System.Collections.Generic;
using System.Text;

namespace MicaForEveryone.Models
{
	public class LogEntry
	{
		public string WindowTitle { get; }

		public LogEntry(bool success, string windowTitle, string windowClass, string processName)
		{
			WindowTitle = windowTitle;
			WindowClass = windowClass;
			ProcessName = processName;
			Success = success;
		}

		public string WindowClass { get; }
		public string ProcessName { get; }
		public DateTime TimeStamp { get; } = DateTime.Now;
		public bool Success { get; }
	}
}
