using HelixToolkit.Wpf.SharpDX;
using IDE.Core;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace IDE.Documents.Views
{
    class RectangleSelectionBehavior : SelectionBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleSelectionCommand" /> class.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="eventHandler">The selection event handler.</param>
        public RectangleSelectionBehavior(FrameworkElement viewport, Func<Rect, IList<ISelectableItem>> funcHitItems)
            : base(viewport)
        {
            if (funcHitItems == null)
                throw new ArgumentNullException(nameof(funcHitItems));

            _funcHitItems = funcHitItems;
        }

        /// <summary>
        /// The selection rectangle.
        /// </summary>
        private Rect selectionRect;

        /// <summary>
        /// The rectangle adorner.
        /// </summary>
        private RectangleAdorner rectangleAdorner;

        private readonly Func<Rect, IList<ISelectableItem>> _funcHitItems;

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        protected override void Started(ManipulationEventArgs e)
        {
            base.Started(e);
           selectionRect = new Rect(MouseDownPoint, MouseDownPoint);
           ShowRectangle();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="ManipulationEventArgs"/> instance containing the event data.</param>
        protected override void Delta(ManipulationEventArgs e)
        {
            base.Delta(e);
            selectionRect = new Rect(MouseDownPoint, e.CurrentPosition);
            UpdateRectangle();

            UpdateSelection();
        }

        void UpdateSelection()
        {
            var canvas = Viewport.DataContext as ICanvasDesignerFileViewModel;
            if (canvas != null)
            {
                var hitItems = _funcHitItems(selectionRect);

                if (hitItems?.Count == 0)
                    return;

                foreach (var item in canvas.Items)
                {
                    var hitItem = hitItems.FirstOrDefault(s => s == item);
                    if (hitItem != null)
                        item.IsSelected = true;
                    else if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        item.IsSelected = false;
                    }
                }
            }
        }

        /// <summary>
        /// The customized complete operation when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        protected override void Completed(ManipulationEventArgs e)
        {
            HideRectangle();
        }



        /// <summary>
        /// Hides the selection rectangle.
        /// </summary>
        private void HideRectangle()
        {
            var myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
            if (rectangleAdorner != null)
            {
                myAdornerLayer.Remove(rectangleAdorner);
            }

            rectangleAdorner = null;

            Viewport.InvalidateVisual();
        }

        /// <summary>
        /// Updates the selection rectangle.
        /// </summary>
        private void UpdateRectangle()
        {
            if (rectangleAdorner == null)
            {
                return;
            }

            rectangleAdorner.Rectangle = selectionRect;
            rectangleAdorner.InvalidateVisual();
        }

        /// <summary>
        /// Shows the selection rectangle.
        /// </summary>
        private void ShowRectangle()
        {
            if (rectangleAdorner != null)
            {
                return;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
            rectangleAdorner = new RectangleAdorner(Viewport, selectionRect, Colors.LightGray, Colors.Transparent, 1, 1, 0, DashStyles.Dash);
            adornerLayer.Add(rectangleAdorner);
        }
    }

}
