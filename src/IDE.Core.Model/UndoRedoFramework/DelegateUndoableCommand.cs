using IDE.Core.Interfaces;
using System;
using System.Windows.Input;

namespace IDE.Core.UndoRedoFramework;

/// <summary>
/// This class implements the ICommand, and the IUndoableAction interface.
/// It relays all its logic to internal delegates that it recieves in its constructor.
/// </summary>
/// <typeparam name="TCommandData">The type of the ICommand members parameter.</typeparam>
/// <typeparam name="TUndoRedoData">The type of the IUndoableAction members parameter.</typeparam>
public sealed class UndoableDelegateCommand<TCommandData, TUndoRedoData> : UndoableDelegateCommandBase<TCommandData, TUndoRedoData>
{
    /// <summary>
    /// Creates a new instance of the UndoableDelegateCommand with the specified delegates providing logic for the class.
    /// </summary>
    /// <param name="execute">A delegate that takes a TCommandData object, and returns a TUndoRedoData object. This delegate is called when the command is executed.</param>
    /// <param name="canExecute">A delegate that takes a TCommandData object and returns a bool. This delegate is called to determine when the command can be executed.</param>
    /// <param name="undo">A delegate that takes a TUndoRedoData object, undoes the actions executed during the execute logic, and returns another TUndoRedoData object to assist in redoing this command.</param>
    /// <param name="redo">A delegate that takes a TUndoRedoData object, redoes the actions executed during the undo logic, and returns another TUndoRedoData object to assist in re-undoing this command.</param>
    /// <param name="name">The name of the command. If left blank, a System.Guid is used.</param>
    /// <param name="context">The context that is used to manage this UndoableCommand</param>
    public UndoableDelegateCommand(Func<TCommandData, object> execute = null,
        Func<TCommandData, bool> canExecute = null,
        Func<object, object> undo = null,
        Func<object, object> redo = null,
        string name = "",
        IUndoRedoContext context = null)
        : base(execute, canExecute, undo, redo, name, context)
    { }

    /// <summary>
    /// Executes this command.
    /// </summary>
    /// <param name="data"></param>
    public void Execute(TCommandData data)
    {
        ((ICommand)this).Execute(data);
    }

    /// <summary>
    /// Checks if this command can execute.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool CanExecute(TCommandData data)
    {
        return ((ICommand)this).CanExecute(data);
    }
}


/// <summary>
/// This class implements the ICommand, and the IUndoableAction interface.
/// It relays all its logic to internal delegates that it recieves in its constructor.
/// </summary>
/// <typeparam name="TUndoRedoData">The type of the IUndoableAction members parameter.</typeparam>
public sealed class UndoableDelegateCommand<TUndoRedoData> : UndoableDelegateCommandBase<object, TUndoRedoData>
{
    /// <summary>
    /// Creates a new instance of the UndoableDelegateCommand with the specified delegates providing logic for the class.
    /// </summary>
    /// <param name="execute">A delegate that returns a TUndoRedoData object. This delegate is called when the command is executed.</param>
    /// <param name="canExecute">A delegate that returns a bool. This delegate is called to determine when the command can be executed.</param>
    /// <param name="undo">A delegate that takes a TUndoRedoData object, undoes the actions executed during the execute logic, and returns another TUndoRedoData object to assist in redoing this command.</param>
    /// <param name="redo">A delegate that takes a TUndoRedoData object, redoes the actions executed during the undo logic, and returns another TUndoRedoData object to assist in re-undoing this command.</param>
    /// <param name="name">The name of the command. If left blank, a System.Guid is used.</param>
    /// <param name="context">The context that is used to manage this UndoableCommand</param>
    public UndoableDelegateCommand(Func<object> execute = null,
        Func<bool> canExecute = null,
        Func<object, object> undo = null,
        Func<object, object> redo = null,
        string name = "",
        IUndoRedoContext context = null)
        : base(
        execute.ValueOrDefault<object>(), 
        canExecute.ValueOrDefault(), 
        undo, 
        redo, 
        name, 
        context)
    {
        
    }

    /// <summary>
    /// Executes this command.
    /// </summary>
    /// <param name="data"></param>
    public void Execute()
    {
        ((ICommand)this).Execute(null);
    }

    /// <summary>
    /// Checks if this command can execute.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool CanExecute()
    {
        return ((ICommand)this).CanExecute(null);
    }
}

/// <summary>
/// This class implements the ICommand, and the IUndoableAction interface.
/// It relays all its logic to internal delegates that it recieves in its constructor.
/// </summary>
public sealed class UndoableDelegateCommand : UndoableDelegateCommandBase<object, object>
{
    /// <summary>
    /// Creates a new instance of the UndoableDelegateCommand with the specified delegates providing logic for the class.
    /// </summary>
    /// <param name="execute">A delegate that performs an action. This delegate is called when the command is executed.</param>
    /// <param name="canExecute">A delegate that returns a bool. This delegate is called to determine when the command can be executed.</param>
    /// <param name="undo">A delegate that takes undoes the actions executed during the execute logic.</param>
    /// <param name="redo">A delegate that redoes the actions executed during the undo logic.</param>
    /// <param name="name">The name of the command. If left blank, a System.Guid is used.</param>
    /// <param name="context">The context that is used to manage this UndoableCommand</param>
    public UndoableDelegateCommand(Action execute = null,
      Func<bool> canExecute = null,
      Action undo = null,
      Action redo = null,
      string name = "",
      IUndoRedoContext context = null)
        : base(
        execute.ValueOrDefault(),
        canExecute.ValueOrDefault(),
        undo.ValueOrDefault(),
        redo.ValueOrDefault(),
        name,
        context) { }

    /// <summary>
    /// Executes this command.
    /// </summary>
    /// <param name="data"></param>
    public void Execute()
    {
        ((ICommand)this).Execute(null);
    }

    /// <summary>
    /// Checks if this command can execute.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool CanExecute()
    {
        return ((ICommand)this).CanExecute(null);
    }
}

internal static class DelegateConversionExtensions
{
    public static Func<object,object> ValueOrDefault(this Action t)
    {
        if (t==null)
        {
            return null;
        }

        return (o) => { t(); return null; }; 

    }
    
    public static Func<object, T> ValueOrDefault<T>(this Func<T> t)
    {
        if (t == null)
        {
            return null;
        }

        return (o) => { return t(); };

    }
    
    public static Func<object, bool> ValueOrDefault(this Func<bool> t)
    {
        if (t == null)
        {
            return null;
        }

        return (o) => { return t(); }; 
    }
}
