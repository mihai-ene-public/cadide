using IDE.Core.UserNotification;
using System;

namespace IDE.Core.Controls
{

    /// <summary>
    /// Implement an interface that enables a viewmodel to interact
    /// with the <seealso cref="InplaceEditBoxLib.Views.EditBox"/> control.
    /// </summary>
    public interface IEditBox //: INotifyableViewModel
    {
        /// <summary>
        /// The viewmodel can fire this event to request editing of its item
        /// name in order to start the rename process via the <seealso cref="InplaceEditBoxLib.Views.EditBox"/> control.
        /// 
        /// The control will fire the command that is bound to the Command dependency
        /// property (if any) with the new name as parameter (if editing was not cancelled
        /// (through escape) in the meantime.
        /// </summary>
        event RequestEditEventHandler RequestEdit;
    }

    /// <summary>
    /// Event handler delegation method to be used when handling <seealso cref="RequestEdit"/> events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void RequestEditEventHandler(object sender, RequestEdit e);

    /// <summary>
    /// Determine the type of edit event that can be reuqested from the view.
    /// </summary>
    public enum RequestEditEvent
    {
        /// <summary>
        /// Start the editing mode for renaming the item represented by the viewmodel
        /// that send this message to its view.
        /// </summary>
        StartEditMode,

        /// <summary>
        /// Unknown type of event should never occur if this enum is used correctly.
        /// </summary>
        Unknown
    }



    /// <summary>
    /// Implements an event that can be send from viewmodel to view
    /// to request certain edit modes.
    /// </summary>
    public class RequestEdit : EventArgs
    {
        #region constructors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        /// <param name="eventRequest"></param>
        public RequestEdit(RequestEditEvent eventRequest) : this()
        {
            Request = eventRequest;
        }

        /// <summary>
        /// Class Constructor
        /// </summary>
        protected RequestEdit()
        {
            Request = RequestEditEvent.Unknown;
        }
        #endregion constructors

        /// <summary>
        /// Gets the type of editing event that was requested by the viewmodel.
        /// </summary>
        public RequestEditEvent Request { get; private set; }
    }
}
