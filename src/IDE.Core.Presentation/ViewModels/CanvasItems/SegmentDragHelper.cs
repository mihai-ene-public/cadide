using System;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Presentation.Utilities;

namespace IDE.Core.Designers
{
    public class SegmentDragHelper
    {
        private readonly IDrawingViewModel _canvasModel;
        private readonly ISegmentedPolylineSelectableCanvasItem _canvasItem;

        public SegmentDragHelper(IDrawingViewModel canvasModel, ISegmentedPolylineSelectableCanvasItem canvasItem)
        {
            _canvasModel = canvasModel;
            _canvasItem = canvasItem;
        }

        private void RunDragStrategy(int segmentIndex, XPoint mousePos)
        {
            switch (_canvasItem)
            {
                case NetWireCanvasItem netWire:
                    RunDragStrategyNetWire(netWire, segmentIndex, mousePos);
                    break;

                case BusWireCanvasItem busWire:
                    RunDragStrategyBusWire(busWire, segmentIndex, mousePos);
                    break;

                case TrackBoardCanvasItem trackWire:
                    RunDragStrategyTrackWire(trackWire, segmentIndex, mousePos);
                    break;
            }
        }

        private void RunDragStrategyNetWire(NetWireCanvasItem canvasItem, int segmentIndex, XPoint mousePos)
        {
            if (!canvasItem.IsSegmentSelected(segmentIndex))
                return;
            var index = segmentIndex;
            var points = canvasItem.Points.ToList();

            if (index == 0)
            {
                //insert a point at the start
                points.Insert(0, points[0]);
                index++;
            }

            if (index == points.Count - 2)
            {
                //insert a point at the end
                points.Add(points.Last());
            }

            var draggedSegment = points.GetSegment(index);
            if (draggedSegment == null)
                return;
            var prevSegment = points.GetSegment(index - 1);
            var nextSegment = points.GetSegment(index + 1);

            var pointOnSegment = draggedSegment.GetPointDistanceToSegment(mousePos);
            var dragDirection = mousePos - pointOnSegment;
            var delta = _canvasModel.SnapToGrid((XPoint)dragDirection);

            var dx = delta.X;
            var dy = delta.Y;

            if (dx == 0.00d && dy == 0.00d)
                return;

            //where we want to be
            var p1X = points[index].X + dx;
            var p1Y = points[index].Y + dy;
            var p2X = points[index + 1].X + dx;
            var p2Y = points[index + 1].Y + dy;

            //keep the same direction for prev and next segments
            var sp = new XPoint(p1X, p1Y);
            var ep = new XPoint(p2X, p2Y);
            var newSeg = new XLineSegment(sp, ep);

            IntersectSegments(ref sp, newSeg, prevSegment);
            IntersectSegments(ref ep, newSeg, nextSegment);

            points[index] = sp;
            points[index + 1] = ep;

            //update the new points
            canvasItem.Points = points;
            canvasItem.SelectSegment(index);
        }

        private void RunDragStrategyBusWire(BusWireCanvasItem canvasItem, int segmentIndex, XPoint mousePos)
        {
            //currently, it is the same code as for a net wire

            if (!canvasItem.IsSegmentSelected(segmentIndex))
                return;
            var index = segmentIndex;
            var points = canvasItem.Points.ToList();

            if (index == 0)
            {
                //insert a point at the start
                points.Insert(0, points[0]);
                index++;
            }

            if (index == points.Count - 2)
            {
                //insert a point at the end
                points.Add(points.Last());
            }

            var draggedSegment = points.GetSegment(index);
            if (draggedSegment == null)
                return;
            var prevSegment = points.GetSegment(index - 1);
            var nextSegment = points.GetSegment(index + 1);

            var pointOnSegment = draggedSegment.GetPointDistanceToSegment(mousePos);
            var dragDirection = mousePos - pointOnSegment;
            var delta = _canvasModel.SnapToGrid((XPoint)dragDirection);

            var dx = delta.X;
            var dy = delta.Y;

            if (dx == 0.00d && dy == 0.00d)
                return;

            //where we want to be
            var p1X = points[index].X + dx;
            var p1Y = points[index].Y + dy;
            var p2X = points[index + 1].X + dx;
            var p2Y = points[index + 1].Y + dy;

            //keep the same direction for prev and next segments
            var sp = new XPoint(p1X, p1Y);
            var ep = new XPoint(p2X, p2Y);
            var newSeg = new XLineSegment(sp, ep);

            IntersectSegments(ref sp, newSeg, prevSegment);
            IntersectSegments(ref ep, newSeg, nextSegment);

            points[index] = sp;
            points[index + 1] = ep;

            //update the new points
            canvasItem.Points = points;
            canvasItem.SelectSegment(index);
        }

        private void RunDragStrategyTrackWire(TrackBoardCanvasItem canvasItem, int segmentIndex, XPoint mousePos)
        {
            //this 1st iteration is the same as net wire and bus wire
            //we need to make it work for 45 degrees
            if (!canvasItem.IsSegmentSelected(segmentIndex))
                return;
            var index = segmentIndex;
            var points = canvasItem.Points.ToList();

            if (index == 0)
            {
                //insert a point at the start
                points.Insert(0, points[0]);
                index++;
            }

            if (index == points.Count - 2)
            {
                //insert a point at the end
                points.Add(points.Last());
            }

            var draggedSegment = points.GetSegment(index);
            if (draggedSegment == null)
                return;
            var prevSegment = points.GetSegment(index - 1);
            var nextSegment = points.GetSegment(index + 1);

            var pointOnSegment = draggedSegment.GetPointDistanceToSegment(mousePos);
            var dragDirection = mousePos - pointOnSegment;
            var delta = _canvasModel.SnapToGrid((XPoint)dragDirection);

            var dx = delta.X;
            var dy = delta.Y;

            if (dx == 0.00d && dy == 0.00d)
                return;

            //where we want to be
            var p1X = points[index].X + dx;
            var p1Y = points[index].Y + dy;
            var p2X = points[index + 1].X + dx;
            var p2Y = points[index + 1].Y + dy;

            //keep the same direction for prev and next segments
            var sp = new XPoint(p1X, p1Y);
            var ep = new XPoint(p2X, p2Y);
            var newSeg = new XLineSegment(sp, ep);

            IntersectSegments(ref sp, newSeg, prevSegment);
            IntersectSegments(ref ep, newSeg, nextSegment);

            points[index] = sp;
            points[index + 1] = ep;

            //update the new points
            canvasItem.Points = points;
            canvasItem.SelectSegment(index);
        }
        //private void RunDragStrategyTrackWire(TrackBoardCanvasItem canvasItem, int segmentIndex, XPoint mousePos)
        //{
        //    if (!canvasItem.IsSegmentSelected(segmentIndex))
        //        return;
        //    var index = segmentIndex;
        //    var points = canvasItem.Points.ToList();
        //    var gridSize = _canvasModel.GridSize;

        //    var guideA = new XLineSegment[2];
        //    var guideB = new XLineSegment[2];

        //    if (index == 0)
        //    {
        //        //insert a point at the start
        //        points.Insert(0, points[0]);
        //        index++;
        //    }

        //    if (index == points.Count - 2)
        //    {
        //        //insert a point at the end
        //        points.Add(points.Last());
        //    }

        //    var draggedSegment = points.GetSegment(index);
        //    if (draggedSegment == null)
        //        return;
        //    var prevSegment = points.GetSegment(index - 1);
        //    var nextSegment = points.GetSegment(index + 1);

        //    var pointOnSegment = draggedSegment.GetPointDistanceToSegment(mousePos);
        //    var dragDirection = mousePos - pointOnSegment;
        //    var delta = _canvasModel.SnapToGrid((XPoint)dragDirection);

        //    var dx = delta.X;
        //    var dy = delta.Y;

        //    if (dx == 0.00d && dy == 0.00d)
        //        return;

        //    //direction of the draged segment
        //    var draggedDir = new XLineDirection(draggedSegment);
        //    var prevDir = new XLineDirection(prevSegment);
        //    var nextDir = new XLineDirection(nextSegment);

        //    //if (prevDir.Direction == MapDirection.Other)
        //    //    prevDir = new XLineDirection(draggedSegment);
        //    //if (nextDir.Direction == MapDirection.Other)
        //    //    nextDir = new XLineDirection(draggedSegment);

        //    if (prevDir.Direction == draggedDir.Direction)
        //    {
        //        prevDir = prevDir.Left();
        //        points.Insert(index, points[index]);
        //        index++;
        //    }
        //    else if (prevDir.Direction == MapDirection.Other)
        //    {
        //        prevDir = prevDir.Left();
        //    }

        //    if (nextDir.Direction == draggedDir.Direction)
        //    {
        //        nextDir = nextDir.Right();
        //        points.Insert(index + 1, points[index + 1]);
        //    }
        //    else if (nextDir.Direction == MapDirection.Other)
        //    {
        //        nextDir = nextDir.Right();
        //    }

        //    draggedSegment = points.GetSegment(index);
        //    prevSegment = points.GetSegment(index - 1);
        //    nextSegment = points.GetSegment(index + 1);

        //    guideA[0] = new XLineSegment(prevSegment.EndPoint, prevSegment.EndPoint + gridSize * prevDir.ToVector());
        //    guideA[1] = new XLineSegment(prevSegment.EndPoint, prevSegment.EndPoint + gridSize * prevDir.ToVector());

        //    if (index == points.Count - 2)
        //    {
        //        guideB[0] = new XLineSegment(draggedSegment.EndPoint, draggedSegment.EndPoint + gridSize * draggedDir.Left().ToVector());
        //        guideB[1] = new XLineSegment(draggedSegment.EndPoint, draggedSegment.EndPoint + gridSize * draggedDir.Right().ToVector());
        //    }
        //    else
        //    {
        //        if (nextDir.IsObtuse(draggedDir))
        //        {
        //            guideB[0] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.Left().ToVector());
        //            guideB[1] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.Right().ToVector());
        //        }
        //        else
        //        {
        //            guideB[0] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.ToVector());
        //            guideB[1] = new XLineSegment(nextSegment.StartPoint, nextSegment.StartPoint + gridSize * nextDir.ToVector());
        //        }
        //    }

        //    int bestLength = int.MaxValue;
        //    var best = new List<XPoint>();

        //    for (int i = 0; i < 2; i++)
        //    {
        //        for (int j = 0; j < 2; j++)
        //        {
        //            var ip1 = draggedSegment.IntersectLines(guideA[i]);
        //            var ip2 = draggedSegment.IntersectLines(guideB[j]);


        //            if (ip1 == null || ip2 == null)
        //                continue;

        //            var np = new List<XPoint>();
        //            var s1 = new XLineSegment(prevSegment.EndPoint, ip1.Value);
        //            var s2 = new XLineSegment(ip1.Value, ip2.Value);
        //            var s3 = new XLineSegment(ip2.Value, nextSegment.StartPoint);

        //            var ip = s1.IntersectLines(nextSegment, false, false);


        //            if (ip != null)
        //            {
        //                np.Add(s1.StartPoint);
        //                np.Add(ip.Value);
        //                np.Add(nextSegment.EndPoint);
        //            }
        //            else
        //            {
        //                ip = s3.IntersectLines(prevSegment, false, false);
        //                if (ip != null)
        //                {
        //                    np.Add(prevSegment.StartPoint);
        //                    np.Add(ip.Value);
        //                    np.Add(s3.EndPoint);
        //                }
        //                else
        //                {
        //                    ip = s1.IntersectLines(s3, false, false);
        //                    if (ip != null)
        //                    {
        //                        np.Add(prevSegment.StartPoint);
        //                        np.Add(ip.Value);
        //                        np.Add(nextSegment.EndPoint);
        //                    }
        //                    else
        //                    {
        //                        np.Add(prevSegment.StartPoint);
        //                        np.Add(ip1.Value);
        //                        np.Add(ip2.Value);
        //                        np.Add(nextSegment.EndPoint);
        //                    }
        //                }
        //            }



        //            if (np.Count < bestLength)
        //            {
        //                bestLength = np.Count;
        //                best = np;

        //                break;
        //            }
        //        }
        //    }

        //    points.Replace(index, index + best.Count - 2, best);
        //    //points.Simplify();

        //    //update the new points
        //    canvasItem.Points = points;
        //    canvasItem.SelectSegment(index);
        //}

        private void IntersectSegments(ref XPoint point, XLineSegment thisSegment, XLineSegment adjacentSeg)
        {
            if (adjacentSeg.IsVertical())
            {
                point.X = adjacentSeg.StartPoint.X;//x1 = x2
            }
            else if (adjacentSeg.IsHorizontal())
            {
                point.Y = adjacentSeg.StartPoint.Y;//y1 = y2
            }
            else
            {
                var pointStart = thisSegment.IntersectLines(adjacentSeg);
                if (pointStart != null)
                {
                    point.X = pointStart.Value.X;
                    point.Y = pointStart.Value.Y;
                }
            }
        }

        public void DragSegment(XPoint mousePos, int segmentIndex)
        {
            if (segmentIndex >= 0)
            {
                RunDragStrategy(segmentIndex, mousePos);
            }
        }
    }
}
