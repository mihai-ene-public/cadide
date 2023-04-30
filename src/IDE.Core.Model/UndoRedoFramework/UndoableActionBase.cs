using IDE.Core.Interfaces;
using System;

namespace IDE.Core.UndoRedoFramework;

/// <summary>
/// This is the base class for all undoable actions.
/// </summary>
public abstract class UndoableActionBase : IUndoableAction
{
    /// <summary>
    /// This constructor prevents external code from inheriting from this class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="context"></param>
    internal UndoableActionBase(string name = "", IUndoRedoContext context = null)
    {
        Context = context;
        if (Context == null)
        {
            Context = UndoRedoContext.GetDefaultContext();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            Name = Guid.NewGuid().ToString();
        }
        else
        {
            Name = name.Trim();
        }

        Context.RegisterAction(this);
    }

    /// <summary>
    /// The optional name for this IUndoableAction
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Stores the context which is managing this UndoableAction.
    /// </summary>
    protected IUndoRedoContext Context { get; set; }
  

    public abstract object Undo(object undoData);

    public abstract object Redo(object redoData);

    public void Dispose()
    {
        Context.UnregisterAction((IUndoableAction)this);
        Context = null;
    }
}
