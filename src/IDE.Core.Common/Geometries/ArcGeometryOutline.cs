using IDE.Core.Types.Media;
using System;

namespace IDE.Core.Common.Geometries
{
    public class ArcGeometryOutline : GeometryOutline
    {
        public ArcGeometryOutline(
                            double centerX,
                            double centerY,
                            double radiusX,
                            double radiusY,
                            double width,
                            double angleStart = 0,
                            double totalAngle = 2 * Math.PI,
                            bool counterClockwise = true,
                            int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle)
        {
            var ellipsePoints = GetEllipsePoints(
                                               new XPoint(centerX, centerY),
                                               numberOfSegmentsPerCircle,
                                               angleStart,
                                               totalAngle,
                                               radiusX: radiusX,
                                               radiusY: radiusY,
                                               counterClockwise: counterClockwise
                                           );

            var outline = GetOutlinedGeometry(ellipsePoints, width);

            points.AddRange(outline);
        }

        public ArcGeometryOutline(
                           double centerX,
                           double centerY,
                           double radius,
                           double width,
                           double angleStart = 0,
                           double totalAngle = 2 * Math.PI,
                           bool counterClockwise = true,
                           int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle)
            : this(centerX, centerY, radius, radius, width, angleStart, totalAngle, counterClockwise, numberOfSegmentsPerCircle)
        {

        }

        public ArcGeometryOutline(XPoint startPoint,
                                  XPoint endPoint,
                                  double radiusX,
                                  double radiusY,
                                  XSweepDirection sweepDirection,
                                  double width,
                                  int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle)
        {
            var ellipsePoints = GetEllipsePoints(
                                             startPoint,
                                             endPoint,
                                             radiusX,
                                             radiusY,
                                             sweepDirection,
                                             numberOfSegmentsPerCircle
                                          );

            var outline = GetOutlinedGeometry(ellipsePoints, 0.5 * width);

            points.AddRange(outline);
        }
    }
}
