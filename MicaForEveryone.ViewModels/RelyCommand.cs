using System;
using System.Windows.Input;

namespace MicaForEveryone.ViewModels
{
    public class RelyCommand : ICommand
    {
        private readonly Action mAction;
        private readonly Func<bool> mCanExecute;

        public RelyCommand(Action action) : this(action, null) { }

        public RelyCommand(Action action, Func<bool> canExecute)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            mAction = action;
            mCanExecute = canExecute;
        }

        public bool CanExecute(object parameter) =>
            mCanExecute == null || mCanExecute();

        public void Execute(object parameter) =>
            mAction();

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    }
}
