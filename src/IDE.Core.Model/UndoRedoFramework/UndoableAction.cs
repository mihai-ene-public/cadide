using IDE.Core.Interfaces;

namespace IDE.Core.UndoRedoFramework;

public class UndoableAction : UndoableActionBase
{
    public UndoableAction(
                        Func<object, object> undo,
                        Func<object, object> redo,
                        string name = "",
                        IUndoRedoContext context = null)
            : base(name, context)
    {
        _UndoFunc = undo;

        _RedoFunc = redo;
    }

    private readonly Func<object, object> _UndoFunc;

    private readonly Func<object, object> _RedoFunc;

    public override object Undo(object undoData)
    {
        return _UndoFunc(undoData);
    }

    public override object Redo(object redoData)
    {
        return _RedoFunc(redoData);
    }
}


