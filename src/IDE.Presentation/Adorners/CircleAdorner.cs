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

namespace IDE.Core.Adorners;

public class CircleAdorner : BaseCanvasItemAdorner
{
    Thumb widthResizeRightThumb;
    Thumb heightResizeBottomThumb;
    Thumb translateThumb;

    XPoint? originalPosition;
    double? originalDiameter;

    ICircleCanvasItem circleItem;

    public CircleAdorner(UIElement adornedElement) : base(adornedElement)
    {
        widthResizeRightThumb = new Thumb
        {
            Cursor = Cursors.SizeWE,
            Width = 1,
            Height = 1,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        widthResizeRightThumb.DragDelta += WidthResizeThumb_DragDelta;
        widthResizeRightThumb.PreviewMouseDown += WidthResizeRightThumb_PreviewMouseDown;
        widthResizeRightThumb.PreviewMouseUp += WidthResizeRightThumb_PreviewMouseUp;

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

        heightResizeBottomThumb = new Thumb
        {
            Cursor = Cursors.SizeNS,
            Width = 1,
            Height = 1,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        heightResizeBottomThumb.DragDelta += HeightResizeThumb_DragDelta;
        heightResizeBottomThumb.PreviewMouseDown += HeightResizeBottomThumb_PreviewMouseDown;
        heightResizeBottomThumb.PreviewMouseUp += HeightResizeBottomThumb_PreviewMouseUp;


        visualChildren.Add(widthResizeRightThumb);
        visualChildren.Add(translateThumb);
        visualChildren.Add(heightResizeBottomThumb);
        circleItem = ((FrameworkElement)AdornedElement).DataContext as ICircleCanvasItem;
        circleItem.PropertyChanged += circleItem_PropertyChanged;
    }

    private void HeightResizeBottomThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newDiameter = circleItem.Diameter;
        var oldDiameter = originalDiameter.Value;
        originalDiameter = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                circleItem.Diameter = oldDiameter;
                return null;
            },
            redo: o =>
            {
                circleItem.Diameter = newDiameter;
                return null;
            },
            null
            );
    }

    private void HeightResizeBottomThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        if (originalDiameter == null)
            originalDiameter = circleItem.Diameter;
    }

    private void TranslateThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newPos = new XPoint(circleItem.X, circleItem.Y);
        var oldPos = originalPosition.Value;
        originalPosition = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                circleItem.X = oldPos.X;
                circleItem.Y = oldPos.Y;
                return null;
            },
            redo: o =>
            {
                circleItem.X = newPos.X;
                circleItem.Y = newPos.Y;
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
            originalPosition = new XPoint(circleItem.X, circleItem.Y);
    }

    private void WidthResizeRightThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newDiameter = circleItem.Diameter;
        var oldDiameter = originalDiameter.Value;
        originalDiameter = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                circleItem.Diameter = oldDiameter;
                return null;
            },
            redo: o =>
            {
                circleItem.Diameter = newDiameter;
                return null;
            },
            null
            );
    }

    private void WidthResizeRightThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        if (originalDiameter == null)
            originalDiameter = circleItem.Diameter;
    }
   

    private void circleItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(circleItem.IsSelected))
        {
            if (!circleItem.IsSelected)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this);
                }
            }
        }
    }

    private void WidthResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (circleItem != null)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

            var width = Math.Abs((circleItem.X - position.X) * 2);
            circleItem.Diameter = width;

            InvalidateVisual();
        }
    }


    private void HeightResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (circleItem != null)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

            var height = Math.Abs((circleItem.Y - position.Y) * 2);
            circleItem.Diameter = height;

            InvalidateVisual();
        }
    }

    private void translateThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (circleItem != null)
        {
            var newLocation = canvasModel.SnapToGridFromDpi(Mouse.GetPosition((IInputElement)VisualTreeHelper.GetParent(AdornedElement)).ToXPoint());

            circleItem.X = newLocation.X;
            circleItem.Y = newLocation.Y;

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

        dc.DrawRectangle(Brushes.White, renderPen, GetTranslationRectangle().ToRect());

        dc.DrawRectangle(Brushes.White, renderPen, GetResizeWidthRightRectangle().ToRect());
        dc.DrawRectangle(Brushes.White, renderPen, GetResizeHeightBottomRectangle().ToRect());
    }

    double GetSize()
    {
        return Math.Max(0.2, circleItem.BorderWidth);
    }
    XRect GetResizeWidthRightRectangle()
    {
        var size = GetSize();
        var startRect = new XRect(circleItem.X + circleItem.Diameter / 2 - size / 2,
                                 circleItem.Y - size / 2,
                                   size,
                                   size);

        return MilimetersToDpiHelper.ConvertToDpi(startRect);
    }

    XRect GetResizeHeightBottomRectangle()
    {
        var size = GetSize();
        return MilimetersToDpiHelper.ConvertToDpi(new XRect(
                       circleItem.X - size / 2,
                       circleItem.Y + circleItem.Diameter / 2 - size / 2,
                        size,
                        size));
    }

    XRect GetTranslationRectangle()
    {
        var size = GetSize();
        return MilimetersToDpiHelper.ConvertToDpi(new XRect(
                        circleItem.X - size  / 2,
                        circleItem.Y - size / 2,
                        size,
                        size));
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
        Geometry g = new RectangleGeometry(GetResizeHeightBottomRectangle().ToRect());
        g = Geometry.Combine(g, new RectangleGeometry(GetResizeWidthRightRectangle().ToRect()), GeometryCombineMode.Union, null);
        g = Geometry.Combine(g, new RectangleGeometry(GetTranslationRectangle().ToRect()), GeometryCombineMode.Union, null);
        return g;
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
        if (circleItem != null)
        {
            var size = Math.Max(1, MilimetersToDpiHelper.ConvertToDpi(circleItem.BorderWidth));
            widthResizeRightThumb.Width = widthResizeRightThumb.Height = size;
            heightResizeBottomThumb.Width = heightResizeBottomThumb.Height = size;
            translateThumb.Width = translateThumb.Height = size;

            widthResizeRightThumb.Arrange(GetResizeWidthRightRectangle().ToRect());
            heightResizeBottomThumb.Arrange(GetResizeHeightBottomRectangle().ToRect());

            translateThumb.Arrange(GetTranslationRectangle().ToRect());
        }

        InvalidateVisual();
    }


}
