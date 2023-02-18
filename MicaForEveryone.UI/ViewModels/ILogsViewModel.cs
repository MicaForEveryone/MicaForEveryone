using MicaForEveryone.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MicaForEveryone.UI.ViewModels
{
	public interface ILogsViewModel
	{
		ObservableCollection<ILogEntryViewModel> Logs { get; }
	}

    public interface ILogEntryViewModel
    {
        bool Success { get; }
        string Header { get; }
        ObservableCollection<LogEntryProperty> Properties { get; }
    }

    public struct LogEntryProperty
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
}
