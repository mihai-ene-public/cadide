using IDE.Core.Types.Media;
using System;
using System.Linq;

namespace IDE.Core.Common.Geometries
{
    public class PathGeometryOutline : GeometryOutline
    {
        public PathGeometryOutline(XPoint startPoint)
        {
            _startPoint = startPoint;

            points.Add(_startPoint);
        }

        XPoint _startPoint;

        public void AddLine(XPoint endPoint)
        {
            points.Add(endPoint);
        }

        public void AddArc(
                            XPoint endPoint,
                            double radiusX,
                            double radiusY,
                            XSweepDirection sweepDirection,
                            int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle
                          )
        {
            if (points.Count == 0)
                throw new Exception("Path was not initialized with a start point");

            var startPoint = points[points.Count - 1];

            var ellipsePoints = GetEllipsePoints(
                                                       startPoint,
                                                       endPoint,
                                                       radiusX,
                                                       radiusY,
                                                       sweepDirection,
                                                       numberOfSegmentsPerCircle
                                                    );

            points.AddRange(ellipsePoints);
        }

        public void AddArc(
                    XPoint endPoint,
                    double radius,
                    XSweepDirection sweepDirection,
                    int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle
                  )
        {
            AddArc(endPoint, radius, radius, sweepDirection, numberOfSegmentsPerCircle);
        }

        public void AddArc(
                             XPoint endPoint,
                             double curvature,
                             int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle
                          )
        {
            var alpha = Math.Abs(0.5 * curvature);
            var sp = points.Last();
            var halfLen = 0.5 * (endPoint - sp).Length;

            var radius = halfLen / Math.Sin(alpha);
            var sweepDir = curvature > 0 ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise;

            AddArc(endPoint, radius, sweepDir, numberOfSegmentsPerCircle);

        }

    }
}
