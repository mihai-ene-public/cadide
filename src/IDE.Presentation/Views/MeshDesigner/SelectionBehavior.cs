using HelixToolkit.Wpf.SharpDX;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    public abstract class SelectionBehavior
    {
        protected SelectionBehavior(FrameworkElement viewport)
        {
            Viewport = viewport;
        }

        protected readonly FrameworkElement Viewport;


        /// <summary>
        /// Gets the mouse down point (2D screen coordinates).
        /// </summary>
        protected Point MouseDownPoint { get; private set; }

        public void Execute()
        {
            OnMouseDown(Viewport);
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void Started(ManipulationEventArgs e)
        {
            MouseDownPoint = e.CurrentPosition;
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void Delta(ManipulationEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void Completed(ManipulationEventArgs e)
        {
        }

       

        /// <summary>
        /// Called when the mouse button is pressed down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        protected virtual void OnMouseDown(object sender)
        {
            Viewport.MouseMove += this.OnMouseMove;
            Viewport.MouseUp += this.OnMouseUp;

            Viewport.Focus();
            Viewport.CaptureMouse();

            Started(new ManipulationEventArgs(Mouse.GetPosition(this.Viewport)));

        }

        /// <summary>
        /// Called when the mouse button is released.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Viewport.MouseMove -= this.OnMouseMove;
            Viewport.MouseUp -= this.OnMouseUp;
            Viewport.ReleaseMouseCapture();
            Completed(new ManipulationEventArgs(Mouse.GetPosition(this.Viewport)));
            e.Handled = true;
        }

        /// <summary>
        /// Called when the mouse is move on the control.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            Delta(new ManipulationEventArgs(Mouse.GetPosition(Viewport)));
            e.Handled = true;
        }
    }

}
