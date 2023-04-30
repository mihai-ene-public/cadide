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
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Core.Adorners;

public class PolygonAdorner : BaseCanvasItemAdorner
{
    List<Thumb> vertexThumbs;

    IPolygonCanvasItem polygon;
    XPoint? originalPosition;

    public PolygonAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        vertexThumbs = new List<Thumb>();

        polygon = ((FrameworkElement)AdornedElement).DataContext as IPolygonCanvasItem;
        polygon.PropertyChanged += Polygon_PropertyChanged;

        var pointsCount = polygon.PolygonPoints.Count;

        for (int vertexIndex = 0; vertexIndex < pointsCount; vertexIndex++)
        {
            var thumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = 4,
                Height = 4,
                Opacity = 0,
                Background = Brushes.Transparent,
                Tag = vertexIndex
            };
            thumb.DragDelta += Thumb_DragDelta;
            thumb.PreviewMouseDown += Thumb_PreviewMouseDown;
            thumb.PreviewMouseUp += Thumb_PreviewMouseUp;

            visualChildren.Add(thumb);
            vertexThumbs.Add(thumb);
        }

    }

    private void Thumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;
        var thumb = sender as Thumb;
        if (thumb == null)
            return;

        var newPos = GetThumbPoint(thumb).Value;
        var oldPos = originalPosition.Value;
        originalPosition = null;

        canvasModel.RegisterUndoActionExecuted(
            undo: o =>
            {
                SetThumbPoint(thumb, ref oldPos);
                return null;
            },
            redo: o =>
            {
                SetThumbPoint(thumb, ref newPos);
                return null;
            },
            null
            );
    }

    private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || canvasModel == null)
            return;

        if (originalPosition == null && sender is Thumb thumb)
            originalPosition = GetThumbPoint(thumb);
    }

    private XPoint? GetThumbPoint(Thumb thumb)
    {
        if (thumb == null)
            return null;

        var vIndex = (int)thumb.Tag;
        return polygon.PolygonPoints[vIndex];
    }

    private void SetThumbPoint(Thumb thumb, ref XPoint point)
    {
        if (thumb == null)
            return;

        var vIndex = (int)thumb.Tag;
        polygon.PolygonPoints[vIndex] = point;

        polygon.OnPropertyChanged(nameof(polygon.PolygonPoints));
        InvalidateVisual();
    }

    void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

        var thumb = sender as Thumb;
        SetThumbPoint(thumb, ref position);
    }

    void Polygon_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(polygon.IsSelected))
        {
            if (!polygon.IsSelected)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(this);
                }
            }
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
        foreach (var vertex in polygon.PolygonPoints)
        {
            var p = MilimetersToDpiHelper.ConvertToDpi(vertex);
            var radius = Math.Max(1.0, MilimetersToDpiHelper.ConvertToDpi(polygon.BorderWidth / 2));

            dc.DrawEllipse(Brushes.White, renderPen, p.ToPoint(), radius, radius);
        }
    }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
        base.OnPreviewMouseDown(e);
        e.Handled = false;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ArrangePolygonItem();

        return base.ArrangeOverride(finalSize);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        var b = base.MeasureOverride(constraint);
        InvalidateVisual();
        return b;
    }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
        var g = Geometry.Empty;
        var radius = Math.Max(1.0, MilimetersToDpiHelper.ConvertToDpi(polygon.BorderWidth / 2) * 1.1);
        foreach (var p in polygon.PolygonPoints)
        {
            var vertex = MilimetersToDpiHelper.ConvertToDpi(p);

            g = Geometry.Combine(g, new EllipseGeometry(vertex.ToPoint(), radius, radius), GeometryCombineMode.Union, null);
        }

        return g;
    }

    void ArrangePolygonItem()
    {
        if (polygon != null)
        {
            foreach (var thumb in vertexThumbs)
            {
                var vertex = MilimetersToDpiHelper.ConvertToDpi(polygon.PolygonPoints[((int)thumb.Tag)]);
                var width = Math.Max(1.0, MilimetersToDpiHelper.ConvertToDpi(polygon.BorderWidth / 2));
                thumb.Width = thumb.Height = width;

                var rect = new Rect(vertex.X - (width / 2), vertex.Y - (width / 2), width, width);

                thumb.Arrange(rect);
            }

            InvalidateVisual();
        }
    }
}
