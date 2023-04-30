using IDE.Core.Converters;
using IDE.Core.Designers;
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

public class ImageAdorner : BaseCanvasItemAdorner
{
    public ImageAdorner(UIElement adornedElement) : base(adornedElement)
    {

        widthResizeThumb = new Thumb
        {
            Cursor = Cursors.SizeWE,
            Width = 4,
            Height = 4,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        widthResizeThumb.DragDelta += WidthResizeThumb_DragDelta;
        widthResizeThumb.PreviewMouseDown += WidthResizeRightThumb_PreviewMouseDown;
        widthResizeThumb.PreviewMouseUp += WidthResizeRightThumb_PreviewMouseUp;

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

        widthAndHeightResizeThumb = new Thumb
        {
            Cursor = Cursors.SizeAll,
            Width = 4,
            Height = 4,
            Opacity = 0,
            Background = Brushes.Transparent,
        };
        widthAndHeightResizeThumb.DragDelta += widthAndHeightResizeThumb_DragDelta;
        widthAndHeightResizeThumb.PreviewMouseDown += WidthAndHeightResizeThumb_PreviewMouseDown;
        widthAndHeightResizeThumb.PreviewMouseUp += WidthAndHeightResizeThumb_PreviewMouseUp;

        visualChildren.Add(widthResizeThumb);
        visualChildren.Add(translateThumb);
        visualChildren.Add(widthAndHeightResizeThumb);
        imageItem = ((FrameworkElement)AdornedElement).DataContext as ImageCanvasItem;
        imageItem.PropertyChanged += imageItem_PropertyChanged;
    }

    Thumb widthResizeThumb;
    Thumb widthAndHeightResizeThumb;
    Thumb translateThumb;

    ImageCanvasItem imageItem;

    XPoint? originalPosition;
    double? originalWidth;
    double? originalHeight;

    private void imageItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(imageItem.IsSelected))
        {
            if (!imageItem.IsSelected)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this);
                }
            }
        }

        InvalidateVisual();
    }

    private void WidthAndHeightResizeThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newHeight = imageItem.Height;
        var newWidth = imageItem.Width;
        var oldHeight = originalHeight.Value;
        var oldWidth = originalWidth.Value;
        originalHeight = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                imageItem.Height = oldHeight;
                imageItem.Width = oldWidth;
                return null;
            },
            redo: o =>
            {
                imageItem.Height = newHeight;
                imageItem.Width = newWidth;
                return null;
            },
            null
            );
    }

    private void WidthAndHeightResizeThumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        if (originalHeight == null && originalWidth == null)
        {
            originalHeight = imageItem.Height;
            originalWidth = imageItem.Width;
        }
    }

    private void TranslateThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newPos = new XPoint(imageItem.X, imageItem.Y);
        var oldPos = originalPosition.Value;
        originalPosition = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                imageItem.X = oldPos.X;
                imageItem.Y = oldPos.Y;
                return null;
            },
            redo: o =>
            {
                imageItem.X = newPos.X;
                imageItem.Y = newPos.Y;
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
            originalPosition = new XPoint(imageItem.X, imageItem.Y);
    }

    private void WidthResizeRightThumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        var newWidth = imageItem.Width;
        var oldWidth = originalWidth.Value;
        originalWidth = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                imageItem.Width = oldWidth;
                return null;
            },
            redo: o =>
            {
                imageItem.Width = newWidth;
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
            originalWidth = imageItem.Width;
    }


    private void WidthResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (imageItem != null)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

            var width = Math.Abs(imageItem.X - position.X);
            imageItem.Width = width;

            InvalidateVisual();
        }
    }


    private void widthAndHeightResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (imageItem != null)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

            var width = Math.Abs(imageItem.X - position.X);
            var height = Math.Abs(imageItem.Y - position.Y);
            imageItem.Width = width;
            imageItem.Height = height;

            InvalidateVisual();
        }
    }

    private void translateThumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (imageItem != null)
        {
            var newLocation = canvasModel.SnapToGridFromDpi(Mouse.GetPosition((IInputElement)VisualTreeHelper.GetParent(AdornedElement)).ToXPoint());

            imageItem.X = newLocation.X;
            imageItem.Y = newLocation.Y;

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

        dc.DrawRectangle(Brushes.White, renderPen, GetResizeWidthRectangle());
        dc.DrawRectangle(Brushes.White, renderPen, GetTranslationRectangle());
        dc.DrawRectangle(Brushes.White, renderPen, GetResizeWidthHeightRectangle());
    }

    double GetSize()
    {
        return Math.Max(0.2, imageItem.BorderWidth);
    }

    Rect GetResizeWidthRectangle()
    {
        var itemWidth = imageItem.Width;
        if (double.IsNaN(itemWidth))
        {
            itemWidth = 100;
        }
        var size = GetSize();
        var startRect = new XRect(itemWidth - size / 2, -size / 2, size, size);
        startRect.X += imageItem.X;
        startRect.Y += imageItem.Y;

        return MilimetersToDpiHelper.ConvertToDpi(startRect).ToRect();
    }

    Rect GetResizeWidthHeightRectangle()
    {
        var itemWidth = imageItem.Width;
        if (double.IsNaN(itemWidth))
        {
            itemWidth = 100;
        }
        var size = GetSize();
        var r = new XRect(itemWidth - size / 2,
                        imageItem.Height - size / 2,
                        size,
                        size);
        r.X += imageItem.X;
        r.Y += imageItem.Y;

        return MilimetersToDpiHelper.ConvertToDpi(r).ToRect();
    }

    Rect GetTranslationRectangle()
    {
        var size = GetSize();
        var r = new XRect(-size / 2, -size / 2, size, size);
        r.X += imageItem.X;
        r.Y += imageItem.Y;

        return MilimetersToDpiHelper.ConvertToDpi(r).ToRect();
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
        Geometry g = new RectangleGeometry(GetResizeWidthRectangle());
        g = Geometry.Combine(g, new RectangleGeometry(GetResizeWidthHeightRectangle()), GeometryCombineMode.Union, null);
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
        if (imageItem != null)
        {
            var size = Math.Max(1, MilimetersToDpiHelper.ConvertToDpi(imageItem.BorderWidth));
            widthResizeThumb.Width = widthResizeThumb.Height = size;
            widthAndHeightResizeThumb.Width = widthAndHeightResizeThumb.Height = size;
            translateThumb.Width = translateThumb.Height = size;

            widthResizeThumb.Arrange(GetResizeWidthRectangle());
            widthAndHeightResizeThumb.Arrange(GetResizeWidthHeightRectangle());

            translateThumb.Arrange(GetTranslationRectangle());
        }

        InvalidateVisual();
    }
}
