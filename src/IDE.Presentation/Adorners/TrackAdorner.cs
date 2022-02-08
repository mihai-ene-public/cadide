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
    //public class TrackAdorner : BaseCanvasItemAdorner
    //{
    //    public TrackAdorner(UIElement adornedElement)
    //        : base(adornedElement)
    //    {

    //        track = ((FrameworkElement)AdornedElement).DataContext as TrackBoardCanvasItem;
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

    //        middleThumb.DragDelta += MiddleDragDelta;
    //        visualChildren.Add(middleThumb);
    //    }

    //    Thumb middleThumb;

    //    TrackBoardCanvasItem track;

    //    // TrackBoardCanvasItem trackCanvasItem;

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

    //    void MiddleDragDelta(object sender, DragDeltaEventArgs e)
    //    {
    //        //TODO: add back for dragging segments

    //        //#if DEBUG
    //        //            if (track != null && track.SelectedSegment >= 0)
    //        //            {

    //        //                DragCurrentSegment();

    //        //                track.OnPropertyChanged(nameof(track.Points));

    //        //                InvalidateVisual();
    //        //            }
    //        //#endif
    //    }

    //    void DragCurrentSegment()
    //    {
    //        var mp = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());
    //        var gridSize = 1.0d;////0.01*  ((IBoardModel)track.LayerDocument).CanvasModel.CanvasGrid.MinorDistance;

    //        var orginalSegments = track.SegmentCount;
    //        var aIndex = track.SelectedSegment;
    //        var index = aIndex;
    //        var guideA = new XLineSegment[2];
    //        var guideB = new XLineSegment[2];

    //        if (index == 0)
    //        {
    //            //insert a point at the start
    //            track.Points.Insert(0, track.Points[0]);
    //            index++;
    //        }

    //        if (index == track.SegmentCount - 1)
    //        {
    //            //insert a point at the end
    //            track.Points.Add(track.Points.Last());
    //        }



    //        var draggedSegment = track.GetSegment(index);
    //        if (draggedSegment == null)
    //            return;
    //        var prevSegment = track.GetSegment(index - 1);
    //        var nextSegment = track.GetSegment(index + 1);

    //        var draggedDir = new XLineDirection(draggedSegment);
    //        var prevDir = new XLineDirection(prevSegment);
    //        var nextDir = new XLineDirection(nextSegment);

    //        if (prevDir.Direction == MapDirection.Other)
    //            prevDir = new XLineDirection(draggedSegment);
    //        if (nextDir.Direction == MapDirection.Other)
    //            nextDir = new XLineDirection(draggedSegment);

    //        if (prevDir.Direction == draggedDir.Direction)
    //        {
    //            prevDir = prevDir.Left();
    //            track.Points.Insert(index, track.Points[index]);
    //            index++;
    //        }
    //        if (nextDir.Direction == draggedDir.Direction)
    //        {
    //            nextDir = nextDir.Right();
    //            track.Points.Insert(index + 1, track.Points[index + 1]);
    //        }

    //        draggedSegment = track.GetSegment(index);
    //        prevSegment = track.GetSegment(index - 1);
    //        nextSegment = track.GetSegment(index + 1);

    //        track.SelectedSegment = index;


    //        if (index == 0)
    //        {

    //            guideA[0] = new XLineSegment(draggedSegment.StartPoint, draggedSegment.StartPoint + gridSize * draggedDir.Left().ToVector());
    //            guideA[1] = new XLineSegment(draggedSegment.StartPoint, draggedSegment.StartPoint + gridSize * draggedDir.Right().ToVector());

    //            //guideA[0] = guideA[1] = new SegmentLine(draggedSegment.StartPoint, draggedSegment.StartPoint + draggedDir.ToVector());
    //        }
    //        else
    //        {
    //            //if (prevDir.IsObtuse(draggedDir))
    //            //{
    //            //    guideA[0] = new SegmentLine(prevSegment.EndPoint, prevSegment.EndPoint + gridSize * prevDir.Left().ToVector());
    //            //    guideA[1] = new SegmentLine(prevSegment.EndPoint, prevSegment.EndPoint + gridSize * prevDir.Right().ToVector());
    //            //}
    //            //else
    //            guideA[0] = guideA[1] = new XLineSegment(prevSegment.EndPoint, prevSegment.EndPoint + gridSize * prevDir.ToVector());
    //        }

    //        if (index == track.SegmentCount - 1)
    //        {
    //            guideB[0] = new XLineSegment(draggedSegment.EndPoint, draggedSegment.EndPoint + gridSize * draggedDir.Left().ToVector());
    //            guideB[1] = new XLineSegment(draggedSegment.EndPoint, draggedSegment.EndPoint + gridSize * draggedDir.Right().ToVector());
    //        }
    //        else
    //        {
    //            if (nextDir.IsObtuse(draggedDir))
    //            {
    //                guideB[0] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.Left().ToVector());
    //                guideB[1] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.Right().ToVector());
    //            }
    //            else
    //                guideB[0] = guideB[1] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.ToVector());
    //        }

    //        var currentSegment = new XLineSegment(mp, mp + draggedDir.ToVector());


    //        int best_len = int.MaxValue;
    //        var best = new List<XPoint>();

    //        for (int i = 0; i < 2; i++)
    //        {
    //            for (int j = 0; j < 2; j++)
    //            {
    //                var ip1 = currentSegment.IntersectLines(guideA[i]);
    //                var ip2 = currentSegment.IntersectLines(guideB[j]);


    //                if (ip1 == null || ip2 == null)
    //                    continue;

    //                var np = new List<XPoint>();
    //                var s1 = new XLineSegment(prevSegment.EndPoint, ip1.Value);
    //                var s2 = new XLineSegment(ip1.Value, ip2.Value);
    //                var s3 = new XLineSegment(ip2.Value, nextSegment.StartPoint);

    //                var ip = s1.IntersectLines(nextSegment, false, false);


    //                if (ip != null)
    //                {
    //                    np.Add(s1.StartPoint);
    //                    np.Add(ip.Value);
    //                    np.Add(nextSegment.EndPoint);
    //                }
    //                else
    //                {
    //                    ip = s3.IntersectLines(prevSegment, false, false);
    //                    if (ip != null)
    //                    {
    //                        np.Add(prevSegment.StartPoint);
    //                        np.Add(ip.Value);
    //                        np.Add(s3.EndPoint);
    //                    }
    //                    else
    //                    {
    //                        ip = s1.IntersectLines(s3, false, false);
    //                        if (ip != null)
    //                        {
    //                            np.Add(prevSegment.StartPoint);
    //                            np.Add(ip.Value);
    //                            np.Add(nextSegment.EndPoint);
    //                        }
    //                        else
    //                        {
    //                            np.Add(prevSegment.StartPoint);
    //                            np.Add(ip1.Value);
    //                            np.Add(ip2.Value);
    //                            np.Add(nextSegment.EndPoint);
    //                        }
    //                    }
    //                }



    //                if (np.Count < best_len)
    //                {
    //                    best_len = np.Count;
    //                    best = np;

    //                    break;
    //                }
    //            }
    //        }

    //        //if (aIndex == 0)
    //        //    track.Replace(0, best.Count - 2, best);
    //        //else if (aIndex == track.SegmentCount - 1)
    //        //    track.Replace(-2, -1, best);
    //        //else
    //        track.Replace(index, index + best.Count - 2, best);

    //        track.Simplify();

    //        if (orginalSegments == track.SegmentCount)
    //        {
    //            //track.SelectedSegment = aIndex;
    //        }
    //        //track.SelectedSegment = aIndex;
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
