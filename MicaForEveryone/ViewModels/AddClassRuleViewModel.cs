using System;
using System.Collections.Generic;
using System.Linq;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.ViewModels
{
    internal class AddClassRuleViewModel : ContentDialogViewModel, IAddClassRuleViewModel
    {
        private string _className = "";

        public AddClassRuleViewModel()
        {
            SubmitCommand = new RelyCommand(DoSubmit, CanSubmit);
            PrimaryCommand = SubmitCommand;

            var classes = new List<string>();
            Window.GetDesktopWindow().ForEachChild(window =>
            {
                classes.Add(window.Class.Name);
            });
            Classes = classes.Distinct().ToArray();
        }

        public string[] Classes { get; }

        public IEnumerable<string> Suggestions => Classes.Where(s => s.StartsWith(ClassName));

        public string ClassName
        {
            get => _className;
            set
            {
                SetProperty(ref _className, value);
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
            return !string.IsNullOrWhiteSpace(ClassName);
        }

        public event EventHandler Submit;
    }
}
