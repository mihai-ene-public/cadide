using System;
using System.Windows.Input;

namespace IDE.Core.Interfaces
{
    public class CommandBindingData
    {
        public CommandBindingData(ICommand command, Action<object> action, Func<object, bool> canExecute, Func<object, bool> handledAction)
        {
            Command = command;
            ExecuteAction = action;
            CanExecuteAction = canExecute;
            HandledAction = handledAction;
        }

        public CommandBindingData(ICommand command, Action<object> action, Func<object, bool> canExecute)
             : this(command, action, canExecute, handledAction: null)
        {

        }

        public CommandBindingData(ICommand command, Action<object> action)
            : this(command, action, canExecute: p => true, handledAction: null)
        {

        }

        public ICommand Command { get; private set; }

        public Action<object> ExecuteAction { get; private set; }

        public Func<object, bool> CanExecuteAction { get; private set; }

        public Func<object, bool> HandledAction { get; private set; }
    }
}
