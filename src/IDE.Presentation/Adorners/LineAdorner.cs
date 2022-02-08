using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Core.Adorners
{
    public class LineAdorner : BaseCanvasItemAdorner
    {
        Thumb startThumb;
        Thumb endThumb;
        Thumb middleThumb;


        ILineCanvasItem wire;

        public LineAdorner(UIElement adornedElement) : base(adornedElement)
        {
            wire = ((FrameworkElement)AdornedElement).DataContext as ILineCanvasItem;
            wire.PropertyChanged += Wire_PropertyChanged;

            var size = MilimetersToDpiHelper.ConvertToDpi(wire.Width);
            startThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = 1,
                Height = 1,
                Opacity = 0,
                Background = Brushes.Transparent
            };
            endThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = 4,
                Height = 4,
                Opacity = 0,
                Background = Brushes.Transparent
            };

            middleThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = size,
                Height = size,
                Opacity = 0,
                Background = Brushes.Transparent
            };

            startThumb.DragDelta += StartDragDelta;
            endThumb.DragDelta += EndDragDelta;
            middleThumb.DragDelta += MiddleDragDelta;

            visualChildren.Add(startThumb);
            //visualChildren.Add(middleThumb);
            visualChildren.Add(endThumb);
        }



        void Wire_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(wire.IsSelected))
            {
                if (!wire.IsSelected)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                    if (adornerLayer != null)
                    {
                        adornerLayer.Remove(this);
                    }
                }
            }
        }

        // Event for the Thumb Start Point
        private void StartDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (wire != null)
            {
                var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

                wire.X1 = position.X;
                wire.Y1 = position.Y;

                InvalidateVisual();
            }
        }

        void MiddleDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (wire != null)
            {
                //middle point will snap to grid but the length should stay the same
                var x1 = MilimetersToDpiHelper.ConvertToDpi(wire.X1);
                var y1 = MilimetersToDpiHelper.ConvertToDpi(wire.Y1);
                var x2 = MilimetersToDpiHelper.ConvertToDpi(wire.X2);
                var y2 = MilimetersToDpiHelper.ConvertToDpi(wire.Y2);
                var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());
                var oldMiddle = new XPoint(0.5 * (x1 + x2), 0.5 * (y1 + y2));
                var delta = (XPoint)(position - oldMiddle);
                //var deltaX = wire.SnapToGrid(delta.X);
                //var deltaY = wire.SnapToGrid(delta.Y);
                delta = canvasModel.SnapToGrid(delta);
                wire.X1 += delta.X;
                wire.Y1 += delta.Y;
                wire.X2 += delta.X;
                wire.Y2 += delta.Y;

                InvalidateVisual();
            }
        }

        // Event for the Thumb End Point
        private void EndDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (wire != null)
            {
                var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

                wire.X2 = position.X;
                wire.Y2 = position.Y;

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

            var startPoint = new Point(MilimetersToDpiHelper.ConvertToDpi(wire.X1),
                                       MilimetersToDpiHelper.ConvertToDpi(wire.Y1));
            var middlePoint = new Point(MilimetersToDpiHelper.ConvertToDpi((wire.X1 + wire.X2) / 2),
                                       MilimetersToDpiHelper.ConvertToDpi((wire.Y1 + wire.Y2) / 2));
            var endPoint = new Point(MilimetersToDpiHelper.ConvertToDpi(wire.X2),
                                       MilimetersToDpiHelper.ConvertToDpi(wire.Y2));
            var radius = MilimetersToDpiHelper.ConvertToDpi(wire.Width / 2);

            dc.DrawEllipse(Brushes.White, null, startPoint, radius, radius);
            //dc.DrawEllipse(Brushes.White, null, middlePoint, radius, radius);
            dc.DrawEllipse(Brushes.White, null, endPoint, radius, radius);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            //to update position of thumbs
            ArrangeWire();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //to update position of thumbs
            ArrangeWire();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            e.Handled = false;
        }

        //protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        //{
        //    base.OnPreviewMouseUp(e);

        //    if (wire is NetWireCanvasItem && e.ChangedButton == MouseButton.Left)
        //    {
        //        var netWire = wire as NetWireCanvasItem;
        //        var mousePos = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

        //        if (mousePos == netWire.StartPoint)
        //            netWire.HandleLinePoint(netWire.StartPoint, canvasModel);
        //        else if (mousePos == netWire.EndPoint)
        //            netWire.HandleLinePoint(netWire.EndPoint, canvasModel);
        //    }
        //}

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeWire();

            var size = base.ArrangeOverride(finalSize);
            return size;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            InvalidateVisual();
            var b = base.MeasureOverride(constraint);//this was before Invalidate

            return b;
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            var x1 = MilimetersToDpiHelper.ConvertToDpi(wire.X1);
            var y1 = MilimetersToDpiHelper.ConvertToDpi(wire.Y1);
            var x2 = MilimetersToDpiHelper.ConvertToDpi(wire.X2);
            var y2 = MilimetersToDpiHelper.ConvertToDpi(wire.Y2);
            var w = MilimetersToDpiHelper.ConvertToDpi(wire.Width) * 1.1;

            //start line cap
            Geometry p = new EllipseGeometry(new Point(x1, y1), w / 2, w / 2);
            //end line cap
            p = Geometry.Combine(p, new EllipseGeometry(new Point(x2, y2), w / 2, w / 2), GeometryCombineMode.Union, null);
            return p;
        }


        void ArrangeWire()
        {
            if (wire != null)
            {
                var x1 = MilimetersToDpiHelper.ConvertToDpi(wire.X1);
                var y1 = MilimetersToDpiHelper.ConvertToDpi(wire.Y1);
                var x2 = MilimetersToDpiHelper.ConvertToDpi(wire.X2);
                var y2 = MilimetersToDpiHelper.ConvertToDpi(wire.Y2);
                var width = MilimetersToDpiHelper.ConvertToDpi(Math.Max(0.2, wire.Width));
                //var height = width;
                startThumb.Width = startThumb.Height = width;
                middleThumb.Width = middleThumb.Height = width;
                endThumb.Width = endThumb.Height = width;

                var startRect = new Rect(x1 - 0.5 * width, y1 - 0.5 * width, width, width);
                startThumb.Arrange(startRect);

                var endRect = new Rect(x2 - 0.5 * width, y2 - 0.5 * width, width, width);
                endThumb.Arrange(endRect);

                var middleRect = new Rect((x2 + x1) / 2 - 0.5 * width,
                                            (y2 + y1) / 2 - 0.5 * width,
                                            width,
                                            width);
                middleThumb.Arrange(middleRect);
            }

            InvalidateVisual();
        }
    }
}
