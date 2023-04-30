using System;

namespace IDE.Core.Interfaces;

/// <summary>
/// Represents an undoable action.
/// </summary>
public interface IUndoableAction : IDisposable
{
    string Name { get; set; }


    object Undo(object data);

    object Redo(object data);
}

//public interface IUndoableAction<T> : IUndoableAction, IDisposable
//{
//    string Name { get; set; }


//    T Undo(T data);

//    T Redo(T data);
//}
