/*See 'NOTICE.txt' for license */
using IDE.Core.Interfaces;
using System;
using System.Windows.Input;

namespace IDE.Core.UndoRedoFramework
{
    /// <summary>
    /// This is the base class for all UndoableDelegateCommands.
    /// </summary>
    /// <typeparam name="TCommandData">The type of the parameter passed to the ICommand members.</typeparam>
    /// <typeparam name="TUndoRedoData">The type of the parameter that this command stores.</typeparam>
    public abstract class UndoableDelegateCommandBase<TCommandData, TUndoRedoData> : UndoableActionBase<TUndoRedoData>, ICommand
    {
        /// <summary>
        /// This internal contructor is designed to prevent external code from continuing this inheritance chain. This should never be called from external code.
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        /// <param name="name"></param>
        /// <param name="context"></param>
        internal UndoableDelegateCommandBase(Func<TCommandData, TUndoRedoData> execute = null,
            Func<TCommandData, bool> canExecute = null,
            Func<TUndoRedoData, TUndoRedoData> undo = null,
            Func<TUndoRedoData, TUndoRedoData> redo = null,
            string name = "",
            IUndoRedoContext context = null)
            : base(name, context)
        {
            this.ExecuteOverride = execute;

            this.CanExecuteOverride = canExecute;

            this.UndoOverride = undo;

            this.RedoOverride = redo;

        }

        #region Type Checks
        /// <summary>
        /// Checks if any given object matches the type of TCommandData.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool CheckCommandDataType(object data)
        {
            if (data == null)
            {
                return true;
            }
            return data is TCommandData;
        }

        /// <summary>
        /// Verifies if any given object matches the type of TCommandData.
        /// </summary>
        /// <param name="data"></param>
        private void VerifyCommandDataType(object data)
        {
            if (!CheckCommandDataType(data))
            {
                throw new GenericTypeMismatchException(typeof(TCommandData), data.GetType());
            }
        }
        #endregion

        #region ICommand Members
        bool ICommand.CanExecute(object parameter)
        {
            VerifyCommandDataType(parameter);
            if (this.ExecuteOverride == null)
            {
                return false;
            }

            if (this.CanExecuteOverride == null)
            {
                return true;
            }

            return this.CanExecuteOverride((TCommandData)parameter);
        }

        public event EventHandler CanExecuteChanged;
        //event EventHandler ICommand.CanExecuteChanged
        //{
        //    add { CommandManager.RequerySuggested += value; }
        //    remove { CommandManager.RequerySuggested -= value; }
        //}

        void OnCanExecuteChanged()
        {
            var h = CanExecuteChanged;
            h?.Invoke(this, EventArgs.Empty);
        }

        void ICommand.Execute(object parameter)
        {
            if (((ICommand)this).CanExecute(parameter))
            {
                var undoData = this.ExecuteOverride((TCommandData)parameter);
                if (!(this.UndoOverride == null || this.RedoOverride == null))
                {
                    base.Context.ActionExecuted(this, undoData);
                }
            }
        }
        #endregion

        /// <summary>
        /// Stores the Execute logic for this command.
        /// </summary>
        Func<TCommandData, TUndoRedoData> ExecuteOverride { get; set; }
        /// <summary>
        /// Stores the CanExecute logic for this command.
        /// </summary>
        Func<TCommandData, bool> CanExecuteOverride { get; set; }
        /// <summary>
        /// Stores the Undo logic for this command. 
        /// </summary>
        Func<TUndoRedoData, TUndoRedoData> UndoOverride { get; set; }
        /// <summary>
        /// Stores the Redo logic for this command.
        /// </summary>
        Func<TUndoRedoData, TUndoRedoData> RedoOverride { get; set; }

        /// <summary>
        /// Overriden from UndoableActionBase.
        /// </summary>
        /// <param name="undoData"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override TUndoRedoData Undo(TUndoRedoData undoData)
        {
            return this.UndoOverride(undoData);
        }
        /// <summary>
        /// Overriden from UndoableActionBase.
        /// </summary>
        /// <param name="redoData"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override TUndoRedoData Redo(TUndoRedoData redoData)
        {
            return this.RedoOverride(redoData);
        }
    }

}
