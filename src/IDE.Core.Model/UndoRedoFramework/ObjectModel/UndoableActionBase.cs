/*See 'NOTICE.txt' for license */
using IDE.Core.Interfaces;
using System;

namespace IDE.Core.UndoRedoFramework
{
    /// <summary>
    /// This is the base class for all undoable actions.
    /// </summary>
    /// <typeparam name="TUndoRedoData"></typeparam>
    public abstract class UndoableActionBase<TUndoRedoData> : IUndoableAction
    {
        /// <summary>
        /// This constructor prevents external code from inheriting from this class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        internal UndoableActionBase(string name = "", IUndoRedoContext context = null)
        {
            this.Context = context;
            if (this.Context == null)
            {
                this.Context = UndoRedoContext.GetDefaultContext();
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                this.Name = Guid.NewGuid().ToString();
            }
            else
            {
                this.Name = name.Trim();
            }

            this.Context.RegisterAction(this);
        }
       
        /// <summary>
        /// Stores the context which is managing this UndoableAction.
        /// </summary>
        protected IUndoRedoContext Context { get; set; }
        #region Type Checks
        /// <summary>
        /// Checks if any given object matches the type of TUndoRedoData.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected bool CheckUndoRedoDataType(object data)
        {
            if (data == null)
            {
                return true;
            }
            return data is TUndoRedoData;
        }
        /// <summary>
        /// Verifies if any given object matches the type of TUndoRedoData.
        /// </summary>
        /// <param name="data"></param>
        protected void VerifyUndoRedoDataType(object data)
        {
            if (!CheckUndoRedoDataType(data))
            {
                throw new GenericTypeMismatchException(typeof(TUndoRedoData), data.GetType()); ;
            }
        }
        #endregion

        #region Protected Abstract Members
        /// <summary>
        /// Requires all inheritors to ovveride this method. Contains the implementation for IUndoableAction.Undo
        /// </summary>
        /// <param name="undoData"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected abstract TUndoRedoData Undo(TUndoRedoData undoData);
        /// <summary>
        /// Requires all inheritors to ovveride this method. Contains the implementation for IUndoableAction.Redo
        /// </summary>
        /// <param name="undoData"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected abstract TUndoRedoData Redo(TUndoRedoData redoData);
        #endregion

        object IUndoableAction.Undo(object data)
        {
            VerifyUndoRedoDataType(data);
            return this.Undo((TUndoRedoData)data);
        }

        object IUndoableAction.Redo(object data)
        {
            VerifyUndoRedoDataType(data);
            return this.Redo((TUndoRedoData)data);
        }


        /// <summary>
        /// The optional name for this IUndoableAction
        /// </summary>
        public string Name { get; set; }

        ~UndoableActionBase()
        {
            this.Context.UnregisterAction(this);
            this.Context = null;
        }
    }

}
