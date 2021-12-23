using System;
using System.Windows.Input;

namespace MicaForEveryone.ViewModels
{
    public class RelyCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;

        public RelyCommand(Action<object> action) : this(action, null) { }

        public RelyCommand(Action<object> action, Func<object, bool> canExecute)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _action(parameter);

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    }
}
