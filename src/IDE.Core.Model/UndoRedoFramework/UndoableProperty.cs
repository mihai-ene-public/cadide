/*See 'NOTICE.txt' for license */
using IDE.Core.Interfaces;
using System;
using System.ComponentModel;

namespace IDE.Core.UndoRedoFramework
{
    /// <summary>
    /// Represents the backing store for a property that is automatically undoable.
    /// </summary>
    /// <typeparam name="TPropertyType"></typeparam>
    public class UndoableProperty<TPropertyType> : UndoableActionBase<TPropertyType>, IUndoableProperty
    {
        /// <summary>
        /// Creates a new UndoableProperty.
        /// </summary>
        /// <param name="associatedObject">The object containing the property.</param>
        /// <param name="propertyName">The name of the property as a string.</param>
        /// <param name="context">The context which manages this UndoableProperty.</param>
        /// <param name="startingValue">The starting value for the property backer.</param>
        public UndoableProperty(INotifyPropertyChanged associatedObject, 
            string propertyName, 
            IUndoRedoContext context = null, 
            TPropertyType startingValue = default(TPropertyType))
            : base(propertyName, context)
        {
            args = new PropertyChangedEventArgs(propertyName);

            if (associatedObject == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                this.associatedObject = associatedObject;
            }
           
            this._internalStore = startingValue;

            this.BatchingTimeout = 1500;
        }


        private PropertyChangedEventArgs args;
        private INotifyPropertyChanged associatedObject;

        /// <summary>
        /// Returns the value this container is holding.
        /// </summary>
        /// <returns></returns>
		public TPropertyType GetValue()
		{
			return this.BackingStore;
		}

        /// <summary>
        /// Sets the value this container is holding.
        /// </summary>
        /// <param name="value"></param>
		public void SetValue(TPropertyType value)
		{
			this.BackingStore = value;
		}

        /// <summary>
        /// The actual store for the value of the backing store.
        /// </summary>
        private TPropertyType _internalStore;
        protected TPropertyType BackingStore
        {
            get { return _internalStore; }
            set
            {
                if (!object.Equals(_internalStore, value))
                {
                    base.Context.ActionExecuted(this, _internalStore);
                    _internalStore = value;
                    this.Context.RaisePropertyChanged(associatedObject , args);
                }
            }
        }

        /// <summary>
        /// Overriden from UndoableActionBase.
        /// </summary>
        /// <param name="undoData"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override TPropertyType Undo(TPropertyType undoData)
        {
            var oldValue = this._internalStore;
            this._internalStore = undoData;
            this.Context.RaisePropertyChanged(associatedObject, args);
            
            return oldValue;
        }

        /// <summary>
        /// Overriden from UndoableActionBase.
        /// </summary>
        /// <param name="redoData"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override TPropertyType Redo(TPropertyType redoData)
        {
            var oldValue = this._internalStore;
            this._internalStore = redoData;
			this.Context.RaisePropertyChanged(associatedObject , args);

            return oldValue;
        }

        /// <summary>
        /// Controls how long before UndoRedoContext starts batching changes in milliseconds.
        /// </summary>
        public int  BatchingTimeout { get; set; }
        
    }

    

}
