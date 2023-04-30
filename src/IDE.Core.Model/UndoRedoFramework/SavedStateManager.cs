using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

//http://wpfundoredo.codeplex.com

namespace IDE.Core.UndoRedoFramework;

public class SavedStateManager : ISavedStateManager
{
    public SavedStateManager(ISnapshotManager originator)
    {
        _originator = originator;
    }

    private ISnapshotManager _originator;

    Stack<ISavedState> undoStack = new Stack<ISavedState>();

    Stack<ISavedState> redoStack = new Stack<ISavedState>();

    public void Backup()
    {
        var newState = _originator.CreateSnapshot();

        undoStack.Push(newState);

        //if we do an action , we will not make a redo
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count == 0)
        {
            return;
        }

        var state = undoStack.Peek();

        var newState = _originator.CreateSnapshot();

        _originator.RestoreFromSnapshot(state);

        redoStack.Push(newState);
        undoStack.Pop();
    }

    public void Redo()
    {
        if (redoStack.Count == 0)
        {
            return;
        }

        var state = redoStack.Peek();

        _originator.RestoreFromSnapshot(state);

        undoStack.Push(state);
        redoStack.Pop();
    }

}

public class GenericCanvasSavedState : ISavedState
{
    public IList<ICanvasItem> CanvasItems { get; set; }
}

public class StringSavedState : ISavedState
{
    public string Data { get; set; }
}

public class BytesSavedState : ISavedState
{
    public byte[] Data { get; set; }
}
