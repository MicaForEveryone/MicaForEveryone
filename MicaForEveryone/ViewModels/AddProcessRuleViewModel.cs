using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.ViewModels
{
    internal class AddProcessRuleViewModel : ContentDialogViewModel, IAddProcessRuleViewModel
    {
        private string _processName = "";

        public AddProcessRuleViewModel()
        {
            PrimaryCommand = new RelayCommand<Dialog>(DoSubmit, CanSubmit);

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
                PrimaryCommand.NotifyCanExecuteChanged();
            }
        }

        private void DoSubmit(Dialog dialog)
        {
            Submit?.Invoke(this, EventArgs.Empty);
            dialog.Close();
        }

        private bool CanSubmit(Dialog _)
        {
            return !string.IsNullOrWhiteSpace(ProcessName);
        }

        public event EventHandler Submit;
    }
}
