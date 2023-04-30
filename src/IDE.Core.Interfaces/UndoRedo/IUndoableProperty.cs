namespace IDE.Core.Interfaces
{
    /// <summary>
    /// Represents an undoable property.
    /// </summary>
    public interface IUndoableProperty<T> : IUndoableAction, IUndoableProperty
    {
    }

    public interface IUndoableProperty
    {
        /// <summary>
        /// Represents the time to wait before batching property changes together. Measured in milliseconds.
        /// </summary>
        int BatchingTimeout { get; set; }
    }
}
