using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.Core.Common
{
    public class GeometryTools
    {
        public static IList<XPoint> GetCircleSegment(XPoint offset,
                                                    int thetaDiv,
                                                    double totalAngle = 2 * Math.PI,
                                                    double angleOffset = 0,
                                                    double radius = 1.0,
                                                    bool counterClockwise = true)
        {
            var points = new List<XPoint>();

            if (counterClockwise)
            {
                for (int i = 0; i < thetaDiv; i++)
                {
                    var theta = totalAngle * ((double)i / (thetaDiv - 1)) + angleOffset;
                    points.Add(new XPoint(offset.X + radius * Math.Cos(theta), offset.Y + radius * Math.Sin(theta)));
                }
            }
            else
            {
                for (int i = thetaDiv - 1; i >= 0; i--)
                {
                    var theta = totalAngle * ((double)i / (thetaDiv - 1)) + angleOffset;
                    points.Add(new XPoint(offset.X + radius * Math.Cos(theta), offset.Y + radius * Math.Sin(theta)));
                }
            }
            return points;
        }

        public static IList<XPoint> GetCirclePoints(XPoint offset,
                                                   int thetaDiv,
                                                   double totalAngle = 2 * Math.PI,
                                                   double angleStart = 0,
                                                   double radius = 1.0,
                                                   bool counterClockwise = true)
        {
            var points = new List<XPoint>();
            var deltaTheta = totalAngle / thetaDiv;
            int direction = counterClockwise ? 1 : -1;

            var theta = angleStart;

            for (int i = 0; i <= thetaDiv; i++)
            {
                points.Add(new XPoint(offset.X + radius * Math.Cos(theta), offset.Y + radius * Math.Sin(theta)));

                theta += direction * deltaTheta;
            }

            return points;
        }

        public static IList<XPoint> GetRectangleSectionPoints(double width, double height, bool close = false)
        {
            var points = new List<XPoint>()
                    {
                        new XPoint(-0.5 * width, -0.5 * height),
                        new XPoint(-0.5 * width,  0.5 * height),
                        new XPoint( 0.5 * width,  0.5 * height),
                        new XPoint( 0.5 * width, -0.5 * height),
                    };

            if (close)
            {
                points.Add(points[0]);
            }

            return points;
        }

        public static IList<XPoint3D> CreateGullWingPathPin(double pathHeight, double upperSegmentLength, double lowerSegmentLength, double bendRadius)
        {
            var path = new List<XPoint3D>();

            if (upperSegmentLength < 0.0d)
                upperSegmentLength = 0.0d;

            path.Add(new XPoint3D(0, 0, pathHeight));

            var bend = GetCirclePoints(new XPoint(upperSegmentLength, -bendRadius + pathHeight), 10, 0.5 * Math.PI, 0.5 * Math.PI, bendRadius, false);

            path.AddRange(bend.Select(p => new XPoint3D(p.X, 0.0d, p.Y)));
            // path.AddRange(bend.Select(p => new XPoint3D(Math.Round(p.X, 4), 0.0d, Math.Round(p.Y, 4))));

            bend = GetCirclePoints(new XPoint(upperSegmentLength + 2 * bendRadius, bendRadius),
                                                  10,
                                                  0.5 * Math.PI,
                                                  Math.PI,
                                                  bendRadius,
                                                  true);

            // path.AddRange(bend.Select(p => new XPoint3D(Math.Round(p.X, 4), 0.0d, Math.Round(p.Y, 4))));
            path.AddRange(bend.Select(p => new XPoint3D(p.X, 0.0d, p.Y)));

            path.Add(new XPoint3D(upperSegmentLength + 2 * bendRadius + lowerSegmentLength, 0, 0));

            path.Reverse();

            return path;
        }

        public static IList<XPoint3D> CreateGullWingPathPin(double pathHeight, double upperSegmentLength, double lowerSegmentLength, double bendRadius, double bendAngleRadians)
        {
            var path = new List<XPoint3D>();

            if (upperSegmentLength < 0.0d)
                upperSegmentLength = 0.0d;

            var thetaDivs = 10;

            path.Add(new XPoint3D(0, 0, pathHeight));

            var bend = GetCirclePoints(new XPoint(upperSegmentLength, -bendRadius + pathHeight),
                                        thetaDivs,
                                        bendAngleRadians,
                                        0.5 * Math.PI,
                                        bendRadius,
                                        false);

            path.AddRange(bend.Select(p => new XPoint3D(p.X, 0.0d, p.Y)));

            // t/x = tg a => x = t / tg a
            var dx = 0.0d;

            if (bendAngleRadians != 0.5 * Math.PI)
            {
                dx = (pathHeight - 2 * bendRadius) / Math.Tan(bendAngleRadians);
            }
            bend = GetCirclePoints(new XPoint(upperSegmentLength + 2 * bendRadius + dx, bendRadius),
                                   thetaDivs,
                                   bendAngleRadians,
                                   Math.PI,
                                   bendRadius,
                                   true);

            path.AddRange(bend.Select(p => new XPoint3D(Math.Round(p.X, 4), 0.0d, Math.Round(p.Y, 4))));

            path.Add(new XPoint3D(upperSegmentLength + 2 * bendRadius + lowerSegmentLength + dx, 0, 0));

            path.Reverse();

            return path;
        }

        public static IList<XPoint3D> CreateTubeWithBending(double pathHeight, double upperSegmentLength, double bendRadius)
        {
            var path = new List<XPoint3D>();

            if (upperSegmentLength < 0.0d)
                upperSegmentLength = 0.0d;

            path.Add(new XPoint3D(0, 0, pathHeight));

            var bend = GetCirclePoints(new XPoint(upperSegmentLength, -bendRadius + pathHeight), 10, 0.5 * Math.PI, 0.5 * Math.PI, bendRadius, false);

            path.AddRange(bend.Select(p => new XPoint3D(p.X, 0.0d, p.Y)));

            path.Add(new XPoint3D(upperSegmentLength + bendRadius, 0, 0));

            path.Reverse();

            return path;
        }

        //copied from GeometryHelper.cs
        public static XPoint GetArcCenter(XPoint startPoint, XPoint endPoint, double radiusX, double radiusY, XSweepDirection sweepDirection)
        {
            //http://www.charlespetzold.com/blog/2008/01/Mathematics-of-ArcSegment.html

            var matx = new XMatrix();
            matx.Scale(radiusY / radiusX, 1);
            var sp = matx.Transform(startPoint);
            var ep = matx.Transform(endPoint);

            // Get info about chord that connects both points
            var midPoint = new XPoint((sp.X + ep.X) / 2, (sp.Y + ep.Y) / 2);
            var vect = ep - sp;
            double halfChord = vect.Length / 2;

            // Get vector from chord to center
            XVector vectRotated;

            var isClockwise = sweepDirection == XSweepDirection.Clockwise;

            if (isClockwise)
                vectRotated = new XVector(-vect.Y, vect.X);
            else
                vectRotated = new XVector(vect.Y, -vect.X);

            vectRotated.Normalize();

            //the larger between radiuses
            var r = Math.Max(radiusX, radiusY);

            // Distance from chord to center 
            double centerDistance = Math.Sqrt(r * r - halfChord * halfChord);

            if (centerDistance.IsNaN())
                centerDistance = 0.0;
            // Calculate center point
            var center = midPoint + centerDistance * vectRotated;
            return center;
        }

    }
}
