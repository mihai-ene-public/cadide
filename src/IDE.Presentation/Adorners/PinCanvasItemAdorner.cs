using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Core.Adorners
{
    public class PinCanvasItemAdorner : BaseCanvasItemAdorner
    {
        public PinCanvasItemAdorner(UIElement adornedElement) : base(adornedElement)
        {
            translateThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = 1,
                Height = 1,
                Opacity = 0,
                Background = Brushes.Transparent,
            };
            translateThumb.DragDelta += translateThumb_DragDelta;
            translateThumb.PreviewMouseDown += TranslateThumb_PreviewMouseDown;
            translateThumb.PreviewMouseUp += TranslateThumb_PreviewMouseUp;

            visualChildren.Add(translateThumb);

            canvasItem = ((FrameworkElement)AdornedElement).DataContext as IPinCanvasItem;
            canvasItem.PropertyChanged += canvasItem_PropertyChanged;
        }

        Thumb translateThumb;

        XPoint? originalPosition;

        IPinCanvasItem canvasItem;

        private void canvasItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(canvasItem.IsSelected))
            {
                if (!canvasItem.IsSelected)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                    if (adornerLayer != null)
                    {
                        adornerLayer.Remove(this);
                    }
                }
            }
        }

        private void TranslateThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || canvasModel == null)
                return;

            var newPos = new XPoint(canvasItem.X, canvasItem.Y);
            var oldPos = originalPosition.Value;
            originalPosition = null;

            canvasModel.RegisterUndoActionExecuted(
                undo: o =>
                {
                    canvasItem.X = oldPos.X;
                    canvasItem.Y = oldPos.Y;
                    return null;
                },
                redo: o =>
                {
                    canvasItem.X = newPos.X;
                    canvasItem.Y = newPos.Y;
                    return null;
                },
                null
                );
        }

        private void TranslateThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || canvasModel == null)
                return;

            if (originalPosition == null)
                originalPosition = new XPoint(canvasItem.X, canvasItem.Y);
        }

        private void translateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (canvasItem != null)
            {
                var newLocation = canvasModel.SnapToGridFromDpi(Mouse.GetPosition((IInputElement)VisualTreeHelper.GetParent(AdornedElement)).ToXPoint());

                canvasItem.X = newLocation.X;
                canvasItem.Y = newLocation.Y;

                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired !
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            Pen renderPen = new Pen(new SolidColorBrush(Colors.White), 0);

            dc.DrawRectangle(Brushes.White, renderPen, GetTranslationRectangle());

        }

        double GetSize()
        {
            return Math.Max(0.2, canvasItem.Width);
        }

        Rect GetTranslationRectangle()
        {
            var size = GetSize();
            return MilimetersToDpiHelper.ConvertToDpi(new XRect(
                            canvasItem.X - size / 2,
                            canvasItem.Y - size / 2,
                            size,
                            size)).ToRect();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            //to update position of thumbs
            ArrangeDesignerItem();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //to update position of thumbs
            ArrangeDesignerItem();
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(GetTranslationRectangle());
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeDesignerItem();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var b = base.MeasureOverride(constraint);
            InvalidateVisual();
            return b;
        }

        void ArrangeDesignerItem()
        {
            if (canvasItem != null)
            {
                var size = GetSize();
                size = MilimetersToDpiHelper.ConvertToDpi(size);
                translateThumb.Width = translateThumb.Height = size;

                translateThumb.Arrange(GetTranslationRectangle());
            }

            InvalidateVisual();
        }
    }
}
