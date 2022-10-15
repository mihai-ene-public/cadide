using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Core
{
    public static class Geometry2DHelper
    {
        public static bool CirclesIntersect(XPoint center1, double radius1, XPoint center2, double radius2)
        {
            var centersDistSquared = (center1 - center2).LengthSquared;
            var radiiSquared = (radius1 + radius2) * (radius1 + radius2);

            return centersDistSquared <= radiiSquared;
        }

        public static bool ArePointsColinear(XPoint p0, XPoint p1, XPoint p2)
        {
            var r = p1 - p0;
            var s = p2 - p1;
            var rxs = Math.Abs(XVector.CrossProduct(r, s));

            var eps = 1e-3;
            return (rxs <= eps);// && qpxr <= eps);
        }

        public static bool PointContinuesAt(XPoint thisPoint, XPoint startPoint, XPoint endPoint)
        {
            return thisPoint == startPoint
               || thisPoint == endPoint;
        }

        /// <summary>
        /// //removes duplicate vertices and colinear adjacent segments
        /// </summary>
        /// <param name="linePoints"></param>
        /// <param name="simplifiedPoints"></param>
        /// <returns></returns>
        public static bool SimplifyPolyline(IList<XPoint> linePoints, IList<XPoint> simplifiedPoints)
        {
            simplifiedPoints.Clear();

            if (linePoints.Count <= 2)
                return false;

            var uniquePoints = new List<XPoint>();
            int i = 0;
            int pointsCount = linePoints.Count;

            // stage 1: eliminate duplicate vertices
            while (i < pointsCount)
            {
                int j = i + 1;

                while (j < pointsCount && linePoints[i] == linePoints[j])
                    j++;

                uniquePoints.Add(linePoints[i]);
                i = j;
            }

            pointsCount = uniquePoints.Count;

            i = 0;

            // stage 2: eliminate collinear segments
            while (i < pointsCount - 2)
            {
                var p0 = uniquePoints[i];
                var p1 = uniquePoints[i + 1];
                //var p2 = pts_unique[i + 2];
                int n = i;

                while (n < pointsCount - 2 && ArePointsColinear(p0, p1, uniquePoints[n + 2]))
                    n++;

                simplifiedPoints.Add(p0);

                if (n > i)
                    i = n;

                if (n == pointsCount)
                {
                    simplifiedPoints.Add(uniquePoints[n - 1]);
                    return true;
                }

                i++;
            }

            if (pointsCount > 1)
                simplifiedPoints.Add(uniquePoints[pointsCount - 2]);

            simplifiedPoints.Add(uniquePoints[pointsCount - 1]);

            return simplifiedPoints.Count < linePoints.Count;//simplified;
        }

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

        public static XRect GetArcBoundingBox(XPoint startPoint, XPoint endPoint, double radius, XSweepDirection sweepDirection)
        {
            var C = GetArcCenter(startPoint, endPoint, radius, radius, sweepDirection);

            // Put the endpoints into the bounding box:
            double x1 = startPoint.X;
            double y1 = startPoint.Y;
            double x2 = x1, y2 = y1;
            if (endPoint.X < x1)
                x1 = endPoint.X;
            if (endPoint.X > x2)
                x2 = endPoint.X;
            if (endPoint.Y < y1)
                y1 = endPoint.Y;
            if (endPoint.Y > y2)
                y2 = endPoint.Y;

            // Now consider the top/bottom/left/right extremities of the circle:
            double thetaStart = Math.Atan2(startPoint.Y - C.Y, startPoint.X - C.X);
            double thetaEnd = Math.Atan2(endPoint.Y - C.Y, endPoint.X - C.X);
            if (AnglesInClockwiseSequence(thetaStart, 0/*right*/, thetaEnd))
            {
                double x = (C.X + radius);
                if (x > x2)
                    x2 = x;
            }
            if (AnglesInClockwiseSequence(thetaStart, Math.PI / 2/*bottom*/, thetaEnd))
            {
                double y = (C.Y + radius);
                if (y > y2)
                    y2 = y;
            }
            if (AnglesInClockwiseSequence(thetaStart, Math.PI/*left*/, thetaEnd))
            {
                double x = (C.X - radius);
                if (x < x1)
                    x1 = x;
            }
            if (AnglesInClockwiseSequence(thetaStart, Math.PI * 3 / 2/*top*/, thetaEnd))
            {
                double y = (C.Y - radius);
                if (y < y1)
                    y1 = y;
            }
            return new XRect(x1, y1, x2 - x1, y2 - y1);


        }

        private static bool AnglesInClockwiseSequence(double theta1, double theta2, double theta3)
        {
            return AngularDiffSigned(theta1, theta2) + AngularDiffSigned(theta2, theta3) < 2 * Math.PI;
        }


        /// <summary>
        /// Returns a number between 0 and 360 degrees, as radians, representing the
        /// angle required to go clockwise from 'theta1' to 'theta2'. If 'theta2' is 
        /// 5 degrees clockwise from 'theta1' then return 5 degrees. If it's 5 degrees
        /// anticlockwise then return 360-5 degrees.
        /// </summary>
        private static double AngularDiffSigned(double theta1, double theta2)
        {
            double dif = theta2 - theta1;
            while (dif >= 2 * Math.PI)
                dif -= 2 * Math.PI;
            while (dif <= 0)
                dif += 2 * Math.PI;
            return dif;
        }

        public static double AreaOfSegment(XPoint[] segment)
        {
            return Math.Abs(segment.Take(segment.Length - 1)
                .Select((p, i) => (segment[i + 1].X - p.X) * (segment[i + 1].Y + p.Y))
                .Sum() * 0.5);
        }

        public static XPoint GetClosestPoint(IEnumerable<XPoint> points, XPoint point)
        {
            var cp = new XPoint(double.PositiveInfinity, double.PositiveInfinity);
            var minDist = double.PositiveInfinity;
            foreach (var p in points)
            {
                var ds = (point - p).LengthSquared;
                if (ds < minDist)
                {
                    cp = p;
                    minDist = ds;
                }
            }

            return cp;
        }

        public static double GetRotationAngleFromMatrix(XMatrix matrix)
        {
            var rads = -Math.Atan2(matrix.M21, matrix.M11);
            var rotAngle = rads * 180 / Math.PI;
            return rotAngle;
        }

        public static XPoint GetPointDistanceToSegment(XPoint P, XPoint A, XPoint B)
        {
            var AP = P - A;       //Vector from A to P   
            var AB = B - A;       //Vector from A to B  

            var magnitudeAB = AB.LengthSquared;     //Magnitude of AB vector (it's length squared)     
            var ABAPproduct = AP * AB;    //The DOT product of a_to_p and a_to_b     
            var distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

            if (distance < 0)     //Check if P projection is over vectorAB     
            {
                return A;

            }
            else if (distance > 1)
            {
                return B;
            }
            else
            {
                return A + AB * distance;
            }


        }

        public static double GetDistanceToSegment(XPoint pt, XPoint p1, XPoint p2)
        {
            XPoint closest;
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            var t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);


            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = p1;
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = p2;
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new XPoint(p1.X + t * dx, p1.Y + t * dy);
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static bool IsPointInPolygon(IList<XPoint> polygon, XPoint testPoint)
        {
            bool result = false;
            int j = polygon.Count - 1;
            for (int i = 0; i < polygon.Count; i++)
            {
                if (( polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y ) || ( polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y ))
                {
                    if (polygon[i].X + ( ( testPoint.Y - polygon[i].Y ) / ( polygon[j].Y - polygon[i].Y ) * ( polygon[j].X - polygon[i].X ) ) < testPoint.X)
                    {
                        result = !result;
                    }
                }

                j = i;
            }

            return result;
        }
    }
}
