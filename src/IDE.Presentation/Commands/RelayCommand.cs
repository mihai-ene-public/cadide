using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace IDE.Core.Commands
{


    public class RelayCommand : ICommand
    {
        #region Properties

        private readonly Action<object> ExecuteAction;
        private readonly Predicate<object> CanExecuteAction;

        #endregion

        public RelayCommand(Action<object> execute)
            : this(execute, _ => true)
        {
        }
        public RelayCommand(Action<object> action, Predicate<object> canExecute)
        {
            ExecuteAction = action;
            CanExecuteAction = canExecute;

            //this.RaiseCanExecuteChangedAction = RaiseCanExecuteChanged;
            //SimpleCommandManager.AddRaiseCanExecuteChangedAction(ref RaiseCanExecuteChangedAction);
        }

        #region Methods

        //~RelayCommand()
        //{
        //    RemoveCommand();
        //}

        //public void RemoveCommand()
        //{
        //    SimpleCommandManager.RemoveRaiseCanExecuteChangedAction(RaiseCanExecuteChangedAction);
        //}


        public bool CanExecute(object parameter)
        {
            return CanExecuteAction(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            ExecuteAction(parameter);

            //  SimpleCommandManager.RefreshCommandStates();
        }

        //public void RaiseCanExecuteChanged()
        //{
        //    var handler = CanExecuteChanged;
        //    if (handler != null)
        //    {
        //        handler(this, new EventArgs());
        //    }
        //}

       // private readonly Action RaiseCanExecuteChangedAction;

        //public event EventHandler CanExecuteChanged;

        #endregion
    }
}
