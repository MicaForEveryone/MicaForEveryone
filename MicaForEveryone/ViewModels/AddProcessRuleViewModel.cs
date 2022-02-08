using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.ViewModels
{
    internal class AddProcessRuleViewModel : ContentDialogViewModel, IAddProcessRuleViewModel
    {
        private string _processName = "";

        public AddProcessRuleViewModel()
        {
            SubmitCommand = new RelyCommand(DoSubmit, CanSubmit);
            PrimaryCommand = SubmitCommand;

            Processes = Process.GetProcesses().Select(process => process.ProcessName)
                .Distinct().ToArray();
        }

        public string[] Processes { get; }

        public IEnumerable<string> Suggestions => Processes.Where(s => s.StartsWith(ProcessName));

        public string ProcessName
        {
            get => _processName;
            set
            {
                SetProperty(ref _processName, value);
                OnPropertyChanged(nameof(Suggestions));
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }

        public RelyCommand SubmitCommand { get; }

        private void DoSubmit(object parameter)
        {
            Submit?.Invoke(this, EventArgs.Empty);
            ((Dialog)parameter).Close();
        }

        private bool CanSubmit(object parameter)
        {
            return !string.IsNullOrWhiteSpace(ProcessName);
        }

        public event EventHandler Submit;
    }
}
