using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Spatial2D;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;

namespace IDE.Core.Presentation.PlacementRouting
{
    /*StopAtObstaclesRoutingBehavior
     * we remove temporarly this class; but we will add it back
    internal class StopAtObstaclesRoutingBehavior : RoutingBehavior
    {
        public StopAtObstaclesRoutingBehavior(TrackRoutingMode routingMode) : base(routingMode)
        {
        }

        public override void PlacementMouseMove(XPoint mousePosition)
        {
            if (trackRoutingMode.PlacementTool == null)
                return;

            var status = trackRoutingMode.PlacementTool.PlacementStatus;

            switch (status)
            {
                case PlacementStatus.Ready:
                    currentSegment.Points[0] = mousePosition;
                    currentSegment.Points[1] = mousePosition;
                    //currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                    currentSegment.Signal = null;
                    RemoveMiddlePoint();
                    break;
                case PlacementStatus.Started:
                    {
                        mousePosition = GetClosestPointBeforeObstacle(mousePosition);//, out bool collided);

                        switch (placementMode)
                        {
                            case TracePlacementMode.Single:
                                RemoveMiddlePoint();
                                ////todo: restrict only if we had collision
                                //if (collided)
                                //    mousePosition = GetReducedPoint(currentSegment.Points[0], mousePosition, currentSegment.Width);
                                currentSegment.Points[1] = mousePosition;
                                //currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                                break;

                            case TracePlacementMode.DiagonalDirect:
                                {
                                    EnsureMiddlePoint();
                                    //diagonal
                                    var diagonalPoint = GetDiagonalDirectPoint(mousePosition, currentSegment.Points[0], currentSegment.Points[2]);
                                    currentSegment.Points[1] = diagonalPoint;
                                    //direct
                                    currentSegment.Points[2] = mousePosition;

                                    //currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                                    break;
                                }

                            case TracePlacementMode.DirectDiagonal:
                                {
                                    EnsureMiddlePoint();
                                    var diagonalPoint = GetDirectDiagonalPoint(mousePosition, currentSegment.Points[0], currentSegment.Points[2]);
                                    //direct
                                    currentSegment.Points[1] = diagonalPoint;

                                    //diagonal
                                    currentSegment.Points[2] = mousePosition;

                                    //  currentSegment.OnPropertyChanged(nameof(currentSegment.Points));
                                    break;
                                }
                        }
                        break;
                    }
            }

            PointsChanged();
        }

        XPoint GetClosestPointBeforeObstacle(XPoint mp)//, out bool collided)
        {
            var targetPoint = mp;

            // collided = false;
            //create our virtual lines, as if we wouldn't collide

            var localTrack = new TrackBoardCanvasItem();
            localTrack.Points[0] = currentSegment.StartPoint;
            localTrack.Width = currentSegment.Width;

            switch (placementMode)
            {
                case TracePlacementMode.Single:
                    localTrack.EndPoint = mp;
                    break;

                case TracePlacementMode.DiagonalDirect:
                    {
                        EnsureMiddlePoint(localTrack);
                        //diagonal
                        var diagonalPoint = GetDiagonalDirectPoint(mp, localTrack.StartPoint, localTrack.EndPoint);
                        localTrack.Points[1] = diagonalPoint;
                        //direct
                        localTrack.Points[2] = mp;
                        break;
                    }

                case TracePlacementMode.DirectDiagonal:
                    {
                        EnsureMiddlePoint(localTrack);
                        var diagonalPoint = GetDirectDiagonalPoint(mp, localTrack.StartPoint, localTrack.EndPoint);
                        //direct
                        localTrack.Points[1] = diagonalPoint;

                        //diagonal
                        localTrack.Points[2] = mp;
                        break;
                    }
            }

            //check collision between these lines and obstacles from the board
            //var thisGeometry = GeometryHelper.GetGeometry(localTrack);
            var lineSegments = GetSegments(localTrack.Points);

            //get all the points from collision and align them on the grid
            var distSq = double.MaxValue;
            var nearestObstacles = trackRoutingMode.GetObstaclesInRectangle(localTrack.GetBoundingRectangle());//GetNearestObstacles(mp, localTrack.Width);
            foreach (var obs in nearestObstacles)
            {
                //if we are on the same net, then ignore
                if (obs.CanvasItem is ISignalPrimitiveCanvasItem s)
                {
                    if (s.Signal != null && currentSegment.Signal != null && s.Signal.Name == currentSegment.Signal.Name)
                        continue;
                }

                var clearance = GetClearanceValue(localTrack, obs.CanvasItem);
                var pen = new Pen(Brushes.Transparent, 2 * clearance);

                var collisionPoints = new List<XPoint>();

                var obstacleGeometry = obs.Geometry.GetWidenedPathGeometry(pen, 0.05, ToleranceType.Absolute);
                var obsPoints = TrackRoutingMode.GetGeometryHelper().GetOutlinePoints(obstacleGeometry);
                var obsSegments = GetSegments(obsPoints, closed: true);

                foreach (var lineSegment in lineSegments)
                {
                    var segmentCollided = false;

                    foreach (var obsSeg in obsSegments)
                    {
                        var p = lineSegment.GetIntersectionPoint(obsSeg);
                        if (p.HasValue)
                        {
                            segmentCollided = true;
                            collisionPoints.Add(p.Value);
                        }
                    }

                    if (segmentCollided)
                        break;
                }


                foreach (var cp in collisionPoints)
                {
                    var gridPoint = cp;//currentSegment.Parent.SnapToGrid(cp);
                    var d = (currentSegment.StartPoint - gridPoint).LengthSquared;
                    //var d = (targetPoint - gridPoint).LengthSquared;
                    if (d < distSq)
                    {
                        distSq = d;
                        mp = gridPoint;
                    }
                }
            }

            //return the closest point
            //mp = currentSegment.Parent.SnapToGrid(mp);
            return mp;
        }

        List<SegmentLine> GetSegments(IList<XPoint> points, bool closed = false)
        {
            var segments = new List<SegmentLine>();
            if (points.Count >= 2)
            {
                var segmentCount = points.Count - 1;
                for (int i = 0; i < segmentCount; i++)
                {
                    var seg = new SegmentLine();
                    seg.StartPoint = points[i];

                    seg.EndPoint = (i == points.Count - 1 && closed) ? points[0] : points[i + 1];

                    segments.Add(seg);
                }
            }
            return segments;
        }

        //returns an endPoint on the same direction reduced by reducedLen 
        XPoint GetReducedPoint(XPoint startPoint, XPoint endPoint, double reducedLen)
        {
            var v = endPoint - startPoint;
            var len = v.Length;
            v.Normalize();

            return trackRoutingMode.CanvasModel.SnapToGrid(startPoint + v * (len - reducedLen));
        }

        /// <summary>
        /// returns the clearance value between items based on board DRC rules
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <returns></returns>
        double GetClearanceValue(ISelectableItem item1, ISelectableItem item2)
        {
            return 0.254d;
        }

        //protected override void Start()
        //{
        //    //BuildObstacles();
        //}
    }
    */
    public class ObstacleItem : ISpatialData
    {
        // public BaseCanvasItem CanvasItem { get; set; }
        public ISelectableItem CanvasItem { get; set; }

        public IGeometryOutline Geometry { get; set; }
        public Envelope Envelope { get; set; }
    }

}
