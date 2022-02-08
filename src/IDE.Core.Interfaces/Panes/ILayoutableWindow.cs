using System;
namespace IDE.Core.Interfaces
{

    /// <summary>
    /// A window that has layout
    /// </summary>
    public interface ILayoutableWindow : IWindow, IWindowWithCommands
    {
        /// <summary>
        /// Standard Closed Window event.
        /// </summary>
        event EventHandler Closed;

    }
}
