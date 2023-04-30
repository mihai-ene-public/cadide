using IDE.Core.Interfaces;
using System;
using System.Windows.Input;

namespace IDE.Core.UndoRedoFramework;

/// <summary>
/// This is the base class for all UndoableDelegateCommands.
/// </summary>
/// <typeparam name="TCommandData">The type of the parameter passed to the ICommand members.</typeparam>
/// <typeparam name="TUndoRedoData">The type of the parameter that this command stores.</typeparam>
public abstract class UndoableDelegateCommandBase<TCommandData, TUndoRedoData> : UndoableActionBase, ICommand
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
    internal UndoableDelegateCommandBase(Func<TCommandData, object> execute = null,
        Func<TCommandData, bool> canExecute = null,
        Func<object, object> undo = null,
        Func<object, object> redo = null,
        string name = "",
        IUndoRedoContext context = null)
        : base(name, context)
    {
       ExecuteOverride = execute;

       CanExecuteOverride = canExecute;

       UndoOverride = undo;

       RedoOverride = redo;

    }

    #region ICommand Members
    bool ICommand.CanExecute(object parameter)
    {
        if (ExecuteOverride == null)
        {
            return false;
        }

        if (CanExecuteOverride == null)
        {
            return true;
        }

        return CanExecuteOverride((TCommandData)parameter);
    }

    public event EventHandler CanExecuteChanged;

    void ICommand.Execute(object parameter)
    {
        if (((ICommand)this).CanExecute(parameter))
        {
            var undoData = ExecuteOverride((TCommandData)parameter);
            if (!(UndoOverride == null || RedoOverride == null))
            {
                base.Context.ActionExecuted((IUndoableAction)this, undoData);
            }
        }
    }
    #endregion

    /// <summary>
    /// Stores the Execute logic for this command.
    /// </summary>
    Func<TCommandData, object> ExecuteOverride { get; set; }
    /// <summary>
    /// Stores the CanExecute logic for this command.
    /// </summary>
    Func<TCommandData, bool> CanExecuteOverride { get; set; }
    /// <summary>
    /// Stores the Undo logic for this command. 
    /// </summary>
    Func<object, object> UndoOverride { get; set; }
    /// <summary>
    /// Stores the Redo logic for this command.
    /// </summary>
    Func<object, object> RedoOverride { get; set; }

    /// <summary>
    /// Overriden from UndoableActionBase.
    /// </summary>
    /// <param name="undoData"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override object Undo(object undoData)
    {
        return UndoOverride(undoData);
    }
    /// <summary>
    /// Overriden from UndoableActionBase.
    /// </summary>
    /// <param name="redoData"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    public override object Redo(object redoData)
    {
        return RedoOverride(redoData);
    }
}
