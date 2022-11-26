using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

using MicaForEveryone.Interfaces;
using MicaForEveryone.Win32;

namespace MicaForEveryone.ViewModels
{
    internal class AddClassRuleViewModel : ContentDialogViewModel, IAddClassRuleViewModel
    {
        private string _className = "";

        public AddClassRuleViewModel()
        {
            PrimaryCommand = new RelayCommand<Dialog>(DoSubmit, CanSubmit);

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
            return !string.IsNullOrWhiteSpace(ClassName);
        }

        public event EventHandler Submit;
    }
}
