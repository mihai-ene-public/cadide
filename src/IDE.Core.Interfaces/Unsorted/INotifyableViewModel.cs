using System;

namespace IDE.Core.UserNotification
{
    /// <summary>
    /// This interface can be used to connect viewmodel with view
    /// when showing notifications that can pop-up over a window
    /// or over all currently visible windows (IsTopMost = true in notification viewmodel)
    /// </summary>
    public interface INotifyableViewModel
    {
        /// <summary>
        /// Expose an event that is triggered when the viewmodel tells its view:
        /// Here is another notification message please show it to the user.
        /// </summary>
        event ShowNotificationEventHandler ShowNotificationMessage;
    }

    /// <summary>
    /// Event handler delegation method to be used when handling <seealso cref="ShowNotificationEvent"/> events.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ShowNotificationEventHandler(object sender, ShowNotificationEvent e);

    /// <summary>
    /// This class is used to message the fact that the sub-system would like to show another notification
    /// to the user.
    /// 
    /// Expectation: The connected view is processing the event and shows a (pop-up) message to the user.
    /// </summary>
    public class ShowNotificationEvent : EventArgs
    {
        #region constructor
        /// <summary>
        ///    Initializes a new instance of the ShowNotificationEvent class.
        /// </summary>
        /// <param name="imageIcon"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        public ShowNotificationEvent(string title,
                                     string message,
                                     object imageIcon)
        {
            Title = title;
            Message = message;
          //  ImageIcon = imageIcon;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Get the title string of notification.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Get message of notification.
        /// </summary>
        public string Message { get; protected set; }

        ///// <summary>
        ///// Get url string to an image resource that represents this type of notification.
        ///// </summary>
        //public BitmapImage ImageIcon { get; protected set; }
        #endregion properties
    }
}
