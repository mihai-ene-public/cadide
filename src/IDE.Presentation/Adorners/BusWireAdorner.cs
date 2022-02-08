using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Presentation.PlacementRouting;
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

namespace IDE.Core.Adorners
{
    //public class BusWireAdorner : BaseCanvasItemAdorner
    //{
    //    public BusWireAdorner(UIElement adornedElement)
    //        : base(adornedElement)
    //    {
    //        track = ((FrameworkElement)AdornedElement).DataContext as BusWireCanvasItem;
    //        track.PropertyChanged += Track_PropertyChanged;

    //        //segment index
    //        var fwkElement = adornedElement as FrameworkElement;
    //        // trackCanvasItem = (TrackBoardCanvasItem)track;

    //        var segments = track.GetSegments();
    //        var pointMM = (XPoint)fwkElement.Tag;

    //        for (int i = 0; i < segments.Count; i++)
    //        {
    //            if (segments[i].IsPointOnLine2(pointMM, track.Width))
    //            {
    //                track.SelectedSegment = i;
    //                break;
    //            }
    //        }

    //        // trackCanvasItem.SelectedSegment = segmentIndex;

    //        var size = MilimetersToDpiHelper.ConvertToDpi(track.Width);

    //        middleThumb = new Thumb
    //        {
    //            Cursor = Cursors.SizeAll,
    //            Width = size,
    //            Height = size,
    //            Opacity = 0,
    //            Background = Brushes.Transparent
    //        };

    //       // middleThumb.DragDelta += MiddleDragDelta;
    //        visualChildren.Add(middleThumb);
    //    }

    //    Thumb middleThumb;

    //    BusWireCanvasItem track;

    //    void Track_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == nameof(track.IsSelected))
    //        {
    //            if (!track.IsSelected)
    //            {
    //                track.SelectedSegment = -1;

    //                var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
    //                if (adornerLayer != null)
    //                {
    //                    adornerLayer.Remove(this);

    //                    track.PropertyChanged -= Track_PropertyChanged;
    //                }
    //            }
    //        }
    //    }

    //    Brush brush = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
    //    protected override void OnRender(DrawingContext dc)
    //    {
    //        base.OnRender(dc);

    //        // without a background the OnMouseMove event would not be fired !
    //        // Alternative: implement a Canvas as a child of this adorner, like
    //        // the ConnectionAdorner does.
    //        dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));


    //        var mp = GetMidPoint();
    //        if (mp == null)
    //            return;

    //        var middlePoint = MilimetersToDpiHelper.ConvertToDpi(mp.Value);
    //        var radius = MilimetersToDpiHelper.ConvertToDpi(track.Width * 0.5);


    //        dc.DrawEllipse(brush, null, middlePoint.ToPoint(), radius, radius);
    //    }

    //    XPoint? GetMidPoint()
    //    {
    //        var segment = track.GetSegment(track.SelectedSegment);
    //        if (segment == null)
    //            return null;

    //        return new XPoint(0.5 * (segment.StartPoint.X + segment.EndPoint.X),
    //                           0.5 * (segment.StartPoint.Y + segment.EndPoint.Y));
    //    }

    //    protected override void OnMouseWheel(MouseWheelEventArgs e)
    //    {
    //        base.OnMouseWheel(e);

    //        //to update position of thumbs
    //        ArrangeWire();
    //    }

    //    protected override void OnMouseMove(MouseEventArgs e)
    //    {
    //        base.OnMouseMove(e);

    //        //to update position of thumbs
    //        ArrangeWire();
    //    }

    //    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    //    {
    //        base.OnPreviewMouseDown(e);
    //        e.Handled = false;

    //        //if the adorner is active, we have a line selected
    //        //check to see if we can deselect this line
    //        if (e.LeftButton == MouseButtonState.Pressed)
    //        {
    //            var isLineHit = false;
    //            HitTestResultCallback resultCallback = delegate (HitTestResult result)
    //            {
    //                var line = result.VisualHit as Polyline;
    //                if (line != null && (result.VisualHit as FrameworkElement).DataContext == track)
    //                {
    //                    isLineHit = true;
    //                    return HitTestResultBehavior.Stop;
    //                }
    //                return HitTestResultBehavior.Continue;
    //            };

    //            VisualTreeHelper.HitTest(AdornedElement, null,
    //               resultCallback,
    //               new PointHitTestParameters(e.GetPosition(this))
    //               );


    //            if (!isLineHit && track.IsSelected)
    //                track.IsSelected = false;
    //        }

    //    }

    //    protected override Size ArrangeOverride(Size finalSize)
    //    {
    //        ArrangeWire();

    //        var size = base.ArrangeOverride(finalSize);
    //        return size;
    //    }

    //    protected override Size MeasureOverride(Size constraint)
    //    {
    //        InvalidateVisual();
    //        var b = base.MeasureOverride(constraint);//this was before Invalidate

    //        return b;
    //    }

    //    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    //    {
    //        var w = 0.5 * MilimetersToDpiHelper.ConvertToDpi(track.Width);

    //        var mp = GetMidPoint();
    //        if (mp == null)
    //            return null;
    //        mp = MilimetersToDpiHelper.ConvertToDpi(mp.Value);

    //        Geometry p = new EllipseGeometry(mp.Value.ToPoint(), w, w);
    //        return p;
    //    }


    //    void ArrangeWire()
    //    {
    //        if (track != null)
    //        {
    //            var width = MilimetersToDpiHelper.ConvertToDpi(Math.Max(0.2, track.Width));
    //            middleThumb.Width = middleThumb.Height = width;

    //            var mp = GetMidPoint();
    //            if (mp == null)
    //                return;

    //            mp = MilimetersToDpiHelper.ConvertToDpi(mp.Value);

    //            var middleRect = new Rect(mp.Value.X - 0.5 * width,
    //                                      mp.Value.Y - 0.5 * width,
    //                                      width,
    //                                      width);
    //            middleThumb.Arrange(middleRect);
    //        }

    //        InvalidateVisual();
    //    }
    //}
}
