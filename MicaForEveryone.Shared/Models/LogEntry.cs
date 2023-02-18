using System;
using System.Collections.Generic;
using System.Text;

namespace MicaForEveryone.Models
{
	public class LogEntry
	{
		public LogEntry(bool success, string windowTitle, string windowClass, string processName, string error)
		{
			WindowTitle = windowTitle;
			WindowClass = windowClass;
			ProcessName = processName;
			Success = success;
            Error = error;
        }

		public string WindowTitle { get; }
		public string WindowClass { get; }
		public string ProcessName { get; }
		public DateTime TimeStamp { get; } = DateTime.Now;
		public bool Success { get; }
		public string Error { get; }
	}
}
