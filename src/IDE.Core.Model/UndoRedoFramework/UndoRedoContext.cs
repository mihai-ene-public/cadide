/*See 'NOTICE.txt' for license */
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

//http://wpfundoredo.codeplex.com

namespace IDE.Core.UndoRedoFramework
{
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
            this.AllActions = new Dictionary<string, WeakReference>();
            this.UndoStack = new Stack<Tuple<string, object>>();
            this.RedoStack = new Stack<Tuple<string, object>>();
            this.lastModified = DateTime.Now;
        }
        protected Dictionary<string, WeakReference> AllActions { get; set; }

        /// <summary>
        /// Registers an action with this context.
        /// </summary>
        /// <param name="action"></param>
        public void RegisterAction(IUndoableAction action)
        {
            AllActions.Add(action.Name, new WeakReference(action));
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
        protected IUndoableAction GetAction(string name)
        {
            var potential = AllActions[name];

            if (potential == null)
            {
                return null;
            }

            if (!potential.IsAlive)
            {
                AllActions.Remove(name);
                return null;
            }

            return (IUndoableAction)potential.Target;

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

            if (action is IUndoableProperty && (this.UndoStack.Count > 0 && this.UndoStack.First().Item1 == action.Name))
            {
                if ((DateTime.Now - lastModified).TotalMilliseconds < (action as IUndoableProperty).BatchingTimeout)
                {
                    possible = this.UndoStack.Pop().Item2;
                }
                else
                {
                    this.lastModified = DateTime.Now;
                }
            }
            else
            {
                this.lastModified = DateTime.Now;
            }

            if (possible == null)
            {
                this.UndoStack.Push(new Tuple<string, object>(action.Name, data));
            }
            else
            {
                this.UndoStack.Push(new Tuple<string, object>(action.Name, possible));
            }

            this.RedoStack.Clear();

        }

        /// <summary>
        /// Tells the context to undo the last action executed.
        /// </summary>
        public void Undo()
        {
            this.GetUndoCommand().Execute(null);

        }
        /// <summary>
        /// Returns a bool representing if this context can undo.
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return this.GetUndoCommand().CanExecute(null);
        }

        /// <summary>
        /// Tells the context to redo the last action undone.
        /// </summary>
        public void Redo()
        {
            this.GetRedoCommand().Execute(null);
        }
        /// <summary>
        /// Returns a bool representing if this context can redo.
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return this.GetRedoCommand().CanExecute(null);
        }

        /// <summary>
        /// Returns this context's undo capabilites in the form of an ICommand.
        /// </summary>
        /// <returns></returns>
        public ICommand GetUndoCommand()
        {
            return new UndoableDelegateCommand(() =>
            {
                var p = this.UndoStack.Peek();

                var cmd = GetAction(p.Item1);

                var redoInfo = cmd.Undo(p.Item2);

                this.RedoStack.Push(new Tuple<string, object>(cmd.Name, redoInfo));
                this.UndoStack.Pop();


            }, () => this.UndoStack.Count > 0, null, null, "$UNDO$", this);
        }

        /// <summary>
        /// Returns this context's undo capabilites in the form of an ICommand.
        /// </summary>
        /// <returns></returns>
        public ICommand GetRedoCommand()
        {
            return new UndoableDelegateCommand(
                () =>
                {
                    var p = this.RedoStack.Peek();

                    var cmd = GetAction(p.Item1);

                    var undoInfo = cmd.Redo(p.Item2);

                    this.UndoStack.Push(new Tuple<string, object>(cmd.Name, undoInfo));
                    this.RedoStack.Pop();

                },
                () => this.RedoStack.Count > 0, null, null, "$REDO$", this);
        }

        /// <summary>
        /// Allows the container object to raise property changed notifications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
		public void RaisePropertyChanged(object sender, string propertyName)
        {
            this.RaisePropertyChanged(sender, new PropertyChangedEventArgs(propertyName));
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
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(sender, args);
            }
        }

        ~UndoRedoContext()
        {
            this.UndoStack.Clear();
            this.RedoStack.Clear();
            this.AllActions.Clear();
        }
    }
}
