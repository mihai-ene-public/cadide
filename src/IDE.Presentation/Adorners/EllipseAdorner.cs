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

public class EllipseAdorner : BaseCanvasItemAdorner
{
    Thumb widthResizeRightThumb;
    Thumb heightResizeBottomThumb;
    Thumb translateThumb;

    IEllipseCanvasItem ellipseItem;

    XPoint? originalPosition;
    double? originalWidth;
    double? originalHeight;


    public EllipseAdorner(UIElement adornedElement) : base(adornedElement)
    {

        widthResizeRightThumb = new Thumb
        {
            Cursor = Cursors.SizeWE,
            Width = 4,
            Height = 4,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        widthResizeRightThumb.DragDelta += WidthResizeThumb_DragDelta;
        widthResizeRightThumb.PreviewMouseDown += WidthResizeRightThumb_PreviewMouseDown;
        widthResizeRightThumb.PreviewMouseUp += WidthResizeRightThumb_PreviewMouseUp;

        translateThumb = new Thumb
        {
            Cursor = Cursors.SizeAll,
            Width = 4,
            Height = 4,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        translateThumb.DragDelta += translateThumb_DragDelta;
        translateThumb.PreviewMouseDown += TranslateThumb_PreviewMouseDown;
        translateThumb.PreviewMouseUp += TranslateThumb_PreviewMouseUp;

        heightResizeBottomThumb = new Thumb
        {
            Cursor = Cursors.SizeNS,
            Width = 4,
            Height = 4,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        heightResizeBottomThumb.DragDelta += HeightResizeThumb_DragDelta;
        heightResizeBottomThumb.PreviewMouseDown += HeightResizeBottomThumb_PreviewMouseDown;
        heightResizeBottomThumb.PreviewMouseUp += HeightResizeBottomThumb_PreviewMouseUp;

        visualChildren.Add(widthResizeRightThumb);
        visualChildren.Add(translateThumb);
        visualChildren.Add(heightResizeBottomThumb);
        ellipseItem = ((FrameworkElement)AdornedElement).DataContext as IEllipseCanvasItem;
        ellipseItem.PropertyChanged += ellipseItem_PropertyChanged;
    }


    protected override int VisualChildrenCount { get { return visualChildren.Count; } }
    protected override Visual GetVisualChild(int index) { return visualChildren[index]; }

    private void ellipseItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ellipseItem.IsSelected))
        {
            if (!ellipseItem.IsSelected)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this);
                }
            }
        }
    }

    private void HeightResizeBottomThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newHeight = ellipseItem.Height;
        var oldHeight = originalHeight.Value;
        originalHeight = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                ellipseItem.Height = oldHeight;
                return null;
            },
            redo: o =>
            {
                ellipseItem.Height = newHeight;
                return null;
            },
            null
            );
    }

    private void HeightResizeBottomThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        if (originalHeight == null)
            originalHeight = ellipseItem.Height;
    }

    private void TranslateThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newPos = new XPoint(ellipseItem.X, ellipseItem.Y);
        var oldPos = originalPosition.Value;
        originalPosition = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                ellipseItem.X = oldPos.X;
                ellipseItem.Y = oldPos.Y;
                return null;
            },
            redo: o =>
            {
                ellipseItem.X = newPos.X;
                ellipseItem.Y = newPos.Y;
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
            originalPosition = new XPoint(ellipseItem.X, ellipseItem.Y);
    }

    private void WidthResizeRightThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newWidth = ellipseItem.Width;
        var oldWidth = originalWidth.Value;
        originalWidth = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                ellipseItem.Width = oldWidth;
                return null;
            },
            redo: o =>
            {
                ellipseItem.Width = newWidth;
                return null;
            },
            null
            );
    }

    private void WidthResizeRightThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        if (originalWidth == null)
            originalWidth = ellipseItem.Width;
    }



    private void WidthResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (ellipseItem != null)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

            var width = Math.Abs((ellipseItem.X - position.X) * 2);
            ellipseItem.Width = width;

            InvalidateVisual();
        }
    }


    private void HeightResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (ellipseItem != null)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

            var height = Math.Abs((ellipseItem.Y - position.Y) * 2);
            ellipseItem.Height = height;

            InvalidateVisual();
        }
    }

    private void translateThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (ellipseItem != null)
        {
            var newLocation = canvasModel.SnapToGridFromDpi(Mouse.GetPosition((IInputElement)VisualTreeHelper.GetParent(AdornedElement)).ToXPoint());

            ellipseItem.X = newLocation.X;
            ellipseItem.Y = newLocation.Y;

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

        dc.DrawRectangle(Brushes.White, renderPen, GetResizeWidthRightRectangle());
        dc.DrawRectangle(Brushes.White, renderPen, GetResizeHeightBottomRectangle());
    }
    double GetSize()
    {
        return Math.Max(0.2, ellipseItem.BorderWidth);
    }
    Rect GetResizeWidthRightRectangle()
    {
        var size = GetSize();
        var startRect = new XRect(ellipseItem.X + ellipseItem.Width / 2 - size / 2,
                                 ellipseItem.Y - size / 2,
                                   size,
                                   size);

        return MilimetersToDpiHelper.ConvertToDpi(startRect).ToRect();
    }

    Rect GetResizeHeightBottomRectangle()
    {
        var size = GetSize();
        return MilimetersToDpiHelper.ConvertToDpi(new XRect(ellipseItem.X - size / 2,
                        ellipseItem.Y + ellipseItem.Height / 2 - size / 2,
                        size,
                        size)).ToRect();
    }

    Rect GetTranslationRectangle()
    {
        var size = GetSize();
        return MilimetersToDpiHelper.ConvertToDpi(new XRect(ellipseItem.X - size / 2,
                        ellipseItem.Y - size / 2,
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

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);
        e.Handled = false;
        // System.Diagnostics.Debug.WriteLine("LineAdorner PreviewMouseDown");

        //if the adorner is active, we have a line selected
        //check to see if we can deselect this line
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            //var isHit = false;
            //HitTestResultCallback resultCallback = delegate (HitTestResult result)
            //{
            //    if (result.VisualHit == widthResizeThumb ||
            //    result.VisualHit == widthAndHeightResizeThumb)
            //    {
            //        isHit = true;
            //        return HitTestResultBehavior.Stop;
            //    }

            //    var textBlock = result.VisualHit as TextBlock;
            //    if (textBlock != null && (result.VisualHit as FrameworkElement).DataContext == imageItem)
            //    {
            //        isHit = true;
            //        return HitTestResultBehavior.Stop;
            //    }
            //    return HitTestResultBehavior.Continue;
            //};

            //VisualTreeHelper.HitTest(AdornedElement, null,
            //   resultCallback,
            //   new PointHitTestParameters(e.GetPosition(this))
            //   );


            //if (!isHit && textItem.IsSelected)
            //    textItem.IsSelected = false;
        }

    }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
        Geometry g = new RectangleGeometry(GetResizeHeightBottomRectangle());
        g = Geometry.Combine(g, new RectangleGeometry(GetResizeWidthRightRectangle()), GeometryCombineMode.Union, null);
        g = Geometry.Combine(g, new RectangleGeometry(GetTranslationRectangle()), GeometryCombineMode.Union, null);
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
        if (ellipseItem != null)
        {
            var size = Math.Max(1, MilimetersToDpiHelper.ConvertToDpi(ellipseItem.BorderWidth));
            widthResizeRightThumb.Width = widthResizeRightThumb.Height = size;
            heightResizeBottomThumb.Width = heightResizeBottomThumb.Height = size;
            translateThumb.Width = translateThumb.Height = size;

            widthResizeRightThumb.Arrange(GetResizeWidthRightRectangle());
            heightResizeBottomThumb.Arrange(GetResizeHeightBottomRectangle());
            translateThumb.Arrange(GetTranslationRectangle());
        }

        InvalidateVisual();
    }
}
