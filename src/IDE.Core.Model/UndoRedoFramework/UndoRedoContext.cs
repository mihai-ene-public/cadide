using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

//http://wpfundoredo.codeplex.com

namespace IDE.Core.UndoRedoFramework;

/// <summary>
/// A class that manages UndoableCommands and UndoableProperties
/// </summary>
public class UndoRedoContext : IUndoRedoContext
{
    private static UndoRedoContext _defaultContext;
    /// <summary>
    /// Gets the default context.
    /// </summary>
    /// <returns></returns>
    public static UndoRedoContext GetDefaultContext()
    {
        if (_defaultContext == null)
        {
            _defaultContext = new UndoRedoContext();
        }
        return _defaultContext;
    }

    /// <summary>
    /// Creates a new UndoRedoContext.
    /// </summary>
    public UndoRedoContext()
    {
        AllActions = new Dictionary<string, IUndoableAction>();
        UndoStack = new Stack<Tuple<string, object>>();
        RedoStack = new Stack<Tuple<string, object>>();
        lastModified = DateTime.Now;
    }
    protected Dictionary<string, IUndoableAction> AllActions { get; set; }

    /// <summary>
    /// Registers an action with this context.
    /// </summary>
    /// <param name="action"></param>
    public void RegisterAction(IUndoableAction action)
    {
        AllActions.Add(action.Name, action);
    }

    /// <summary>
    /// Unregisters an action from this context.
    /// </summary>
    /// <param name="action"></param>
    public void UnregisterAction(IUndoableAction action)
    {
        AllActions.Remove(action.Name);
    }

    /// <summary>
    /// Method used internally to get an IUndoableAction given a string.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private IUndoableAction GetAction(string name)
    {
        var action = AllActions[name];

        return action;
    }

    /// <summary>
    /// Keeps track of all actions executed.
    /// </summary>
    protected Stack<Tuple<string, object>> UndoStack { get; set; }
    /// <summary>
    /// Keeps track of all actions undone.
    /// </summary>
    protected Stack<Tuple<string, object>> RedoStack { get; set; }

    /// <summary>
    /// Records when a UndoableProperty has last been modified.
    /// </summary>
    private DateTime lastModified;

    /// <summary>
    /// Tells the context that an action has occurred.
    /// </summary>
    /// <param name="action">The action that was executed.</param>
    /// <param name="data">The data associated with the action.</param>
    public void ActionExecuted(IUndoableAction action, object data)
    {
        object possible = null;

        if (action is IUndoableProperty && (UndoStack.Count > 0 && UndoStack.First().Item1 == action.Name))
        {
            if ((DateTime.Now - lastModified).TotalMilliseconds < (action as IUndoableProperty).BatchingTimeout)
            {
                possible = UndoStack.Pop().Item2;
            }
            else
            {
                lastModified = DateTime.Now;
            }
        }
        else
        {
            lastModified = DateTime.Now;
        }

        if (possible == null)
        {
            UndoStack.Push(new Tuple<string, object>(action.Name, data));
        }
        else
        {
            UndoStack.Push(new Tuple<string, object>(action.Name, possible));
        }

        RedoStack.Clear();

    }

    /// <summary>
    /// Tells the context to undo the last action executed.
    /// </summary>
    public void Undo()
    {
        if (!CanUndo())
            return;

        var p = UndoStack.Peek();

        var cmd = GetAction(p.Item1);

        var redoInfo = cmd.Undo(p.Item2);

        RedoStack.Push(new Tuple<string, object>(cmd.Name, redoInfo));
        UndoStack.Pop();

    }
    /// <summary>
    /// Returns a bool representing if this context can undo.
    /// </summary>
    /// <returns></returns>
    public bool CanUndo()
    {
        return UndoStack.Count > 0;
    }

    /// <summary>
    /// Tells the context to redo the last action undone.
    /// </summary>
    public void Redo()
    {
        if (!CanRedo())
            return;

        var p = RedoStack.Peek();

        var cmd = GetAction(p.Item1);

        var undoInfo = cmd.Redo(p.Item2);

        UndoStack.Push(new Tuple<string, object>(cmd.Name, undoInfo));
        RedoStack.Pop();
    }
    /// <summary>
    /// Returns a bool representing if this context can redo.
    /// </summary>
    /// <returns></returns>
    public bool CanRedo()
    {
        return RedoStack.Count > 0;
    }

    /// <summary>
    /// Allows the container object to raise property changed notifications
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void RaisePropertyChanged(object sender, string propertyName)
    {
        RaisePropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// This event handler provides a way for this context to raise property changed notifications on behalf of the UndoableProperties.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Allows the container object to raise property changed notifications
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void RaisePropertyChanged(object sender, PropertyChangedEventArgs args)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(sender, args);
        }
    }

    public void Dispose()
    {
        UndoStack.Clear();
        RedoStack.Clear();
        AllActions.Clear();
    }
}
