using IDE.Core.Types.Media;

namespace IDE.Core.Common.Geometries
{
    public class EllipseGeometryOutline : GeometryOutline
    {
        public EllipseGeometryOutline(double centerX, double centerY, double radiusX, double radiusY, int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle)
        {
            points.AddRange(GetEllipsePoints(
                                               new XPoint(centerX, centerY),
                                               numberOfSegmentsPerCircle,
                                               radiusX: radiusX,
                                               radiusY: radiusY,
                                               counterClockwise: false
                                           ));
        }

        /// <summary>
        /// constructs a circle
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="radius"></param>
        /// <param name="numberOfSegmentsPerCircle"></param>
        public EllipseGeometryOutline(double centerX, double centerY, double radius, int numberOfSegmentsPerCircle = DefaultNumberOfSegmentsPerCircle)
            : this(centerX, centerY, radius, radius, numberOfSegmentsPerCircle)
        {

        }
    }
}
