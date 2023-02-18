using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            Logs = new ObservableCollection<UI.ViewModels.ILogEntryViewModel>(
                _logger.GetLogEntries().Select(entry => new LogEntryViewModel(entry)));
            _logger.LogEntryAdded += Logger_LogEntryAdded;
        }

        private void Logger_LogEntryAdded(LogEntry obj)
        {
            Logs.Add(new LogEntryViewModel(obj));
        }

        public ObservableCollection<UI.ViewModels.ILogEntryViewModel> Logs { get; }
    }

    internal class LogEntryViewModel : UI.ViewModels.ILogEntryViewModel
    {
        public LogEntryViewModel(LogEntry logEntry)
        {
            Success = logEntry.Success;
            Header = $"{logEntry.TimeStamp} - {(Success ? logEntry.WindowTitle : logEntry.Error)}";
            Properties = new ObservableCollection<UI.ViewModels.LogEntryProperty>
            {
                new() { PropertyName = "Title", Value = logEntry.WindowTitle },
                new() { PropertyName = "Class", Value = logEntry.WindowClass },
                new() { PropertyName = "Process", Value = logEntry.ProcessName },
            };
        }

        public bool Success { get; }
        public string Header { get; }
        public ObservableCollection<UI.ViewModels.LogEntryProperty> Properties { get; }
    }
}
