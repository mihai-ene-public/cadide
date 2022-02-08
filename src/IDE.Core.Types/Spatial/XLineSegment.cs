using System;

namespace IDE.Core.Types.Media
{
    public class XLineSegment
    {
        const double Epsilon = 1e-4;


        public XLineSegment(XPoint startPoint, XPoint endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public XPoint StartPoint { get; set; }

        public XPoint EndPoint { get; set; }

        public XPoint? GetIntersectionPoint(XLineSegment other)
        {
            FindIntersection(StartPoint, EndPoint, other.StartPoint, other.EndPoint, out bool linesIntersect, out bool segmentsIntersect, out XPoint intersection, out XPoint closePoint, out XPoint closeP2);

            if (segmentsIntersect)
                return intersection;

            return null;
        }

        public bool IsHorizontal()
        {
            var dir = new XLineDirection(this);
            return dir.Direction == MapDirection.E || dir.Direction == MapDirection.W;
        }

        public bool IsVertical()
        {
            var dir = new XLineDirection(this);
            return dir.Direction == MapDirection.N || dir.Direction == MapDirection.S;
        }

        /// <summary>
        ///   Find the point of intersection between the lines p1 --> p2 and p3 --> p4.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="linesIntersect">True if the lines containing the segments intersect </param>
        /// <param name="segmentsIntersect"> True if the segments intersect </param>
        /// <param name="intersectionPoint">The point where the lines intersect </param>
        /// <param name="closestP1">The point on the first segment that is closest to the point of intersection</param>
        /// <param name="closestP2">The point on the second segment that is closest to the point of intersection</param>
        private void FindIntersection(
            XPoint p1, XPoint p2, XPoint p3, XPoint p4,
            out bool linesIntersect, out bool segmentsIntersect,
            out XPoint intersectionPoint,
            out XPoint closestP1, out XPoint closestP2)
        {
            //http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/


            // Get the segments' parameters.
            var dx12 = p2.X - p1.X;
            var dy12 = p2.Y - p1.Y;
            var dx34 = p4.X - p3.X;
            var dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            var denominator = (dy12 * dx34 - dx12 * dy34);

            var t1 = ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34) / denominator;

            if (double.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                linesIntersect = false;
                segmentsIntersect = false;
                intersectionPoint = new XPoint(double.NaN, double.NaN);
                closestP1 = new XPoint(double.NaN, double.NaN);
                closestP2 = new XPoint(double.NaN, double.NaN);
                return;
            }
            linesIntersect = true;

            var t2 = ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12) / -denominator;

            // Find the point of intersection.
            intersectionPoint = new XPoint(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segmentsIntersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            closestP1 = new XPoint(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            closestP2 = new XPoint(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        /**
 * Check if bounding boxes do intersect. If one bounding box
 * touches the other, they do intersect.
 * @param a first bounding box
 * @param b second bounding box
 * @return <code>true</code> if they intersect,
 *         <code>false</code> otherwise.
 */
        bool doBoundingBoxesIntersect(XPoint[] a, XPoint[] b)
        {
            return a[0].X <= b[1].X
                && a[1].X >= b[0].X
                && a[0].Y <= b[1].Y
                && a[1].Y >= b[0].Y;
        }

        double crossProduct(XPoint a, XPoint b)
        {
            return a.X * b.Y - b.X * a.Y;
        }

        public bool IsPointOnLine(XPoint b, double maxDistance = Epsilon)
        {
            // Move the image, so that a.first is on (0|0)
            var aTmp = new XLineSegment(new XPoint(0, 0), new XPoint(
                    EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y));
            var bTmp = new XPoint(b.X - StartPoint.X, b.Y - StartPoint.Y);
            double r = crossProduct(aTmp.EndPoint, bTmp);
            return Math.Abs(r) <= maxDistance; //Epsilon;
        }

        public bool IsPointOnLine2(XPoint b, double maxDistance = Epsilon)
        {
            if (isInRange(b.X, StartPoint.X, EndPoint.X, maxDistance) && isInRange(b.Y, StartPoint.Y, EndPoint.Y, maxDistance))
            {
                if ((EndPoint.X - StartPoint.X) <= maxDistance) // Vertical line.
                {
                    return true;
                }

                var M = (EndPoint.Y - StartPoint.Y) / (EndPoint.X - StartPoint.X); // Slope
                var C = -(M * StartPoint.X) + StartPoint.Y; // Y intercept

                // Checking if (x, y) is on the line passing through the end points.
                var dy = b.Y - (M * b.X + C);
                return Math.Abs(dy) <= maxDistance;
            }

            return false;
        }

        bool isInRange(double x, double bound1, double bound2, double tolerance)
        {
            // Handles cases when 'bound1' is greater than 'bound2' and when
            // 'bound2' is greater than 'bound1'.
            return (((x >= (bound1 - tolerance)) && (x <= (bound2 + tolerance))) ||
               ((x >= (bound2 - tolerance)) && (x <= (bound1 + tolerance))));
        }

        public bool IsPointRightOfLine(XPoint b)
        {
            var aTmp = new XLineSegment(new XPoint(0, 0), new XPoint(
                    EndPoint.X - StartPoint.X, EndPoint.Y - StartPoint.Y));
            var bTmp = new XPoint(b.X - StartPoint.X, b.Y - StartPoint.Y);

            return crossProduct(aTmp.EndPoint, bTmp) < 0;
        }

        bool lineSegmentTouchesOrCrossesLine(XLineSegment b)
        {
            return IsPointOnLine(b.StartPoint)
                    || IsPointOnLine(b.EndPoint)
                    || (IsPointRightOfLine(b.StartPoint) ^
                        IsPointRightOfLine(b.EndPoint));
        }

        //public bool Intersects(XLineSegment b)
        //{
        //    return lineSegmentTouchesOrCrossesLine(b)
        //    && b.lineSegmentTouchesOrCrossesLine(this);
        //}

        public bool Intersects(XLineSegment segment)
        {
            FindIntersection(StartPoint, EndPoint, segment.StartPoint, segment.EndPoint, out bool linesIntersect, out bool segmentsIntersect, out XPoint intersection, out XPoint closePoint, out XPoint closeP2);

            return segmentsIntersect;
        }

        public bool Intersects(XRect rect)
        {
            var sp = StartPoint;
            var ep = EndPoint;
            if (rect.Contains(sp) || rect.Contains(ep))
                return true;

            return Intersects(new XLineSegment(rect.TopLeft, rect.TopRight))
                || Intersects(new XLineSegment(rect.TopRight, rect.BottomRight))
                || Intersects(new XLineSegment(rect.BottomLeft, rect.BottomRight))
                || Intersects(new XLineSegment(rect.TopLeft, rect.BottomLeft))
                ;
        }

        /// <summary>
        /// Returns a projected point on this segment from a given point
        /// <para>If projection is outside segment endpoints then one of the endpoints is returned</para>
        /// </summary>
        public XPoint GetPointDistanceToSegment(XPoint point)
        {
            //https://en.wikipedia.org/wiki/Vector_projection

            var sp = StartPoint;
            var ep = EndPoint;
            var AP = point - sp;        //Vector from startPoint to Point   
            var segVector = ep - sp;    //Vector from startPoint to endPoint  

            var vectorMagnitude = segVector.LengthSquared;     //Magnitude of AB vector (it's length squared)     
            var dotproduct = AP * segVector;    //The DOT product of a_to_p and a_to_b     
            var distance = dotproduct / vectorMagnitude; //The normalized "distance" from a to your closest point  

            if (distance < 0 || vectorMagnitude == 0.00d)     //Check if P projection is over vectorAB     
            {
                return sp;
            }
            else if (distance > 1)
            {
                return ep;
            }

            return sp + distance * segVector;
        }

        /**
 * Computes intersection point of segment (this) with segment aSeg.
 * @param aSeg: segment to intersect with
 * @param aIgnoreEndpoints: don't treat corner cases (i.e. end of one segment touching the
 * other) as intersections.
 * @param aLines: treat segments as infinite lines
 * @return intersection point, if exists
 */
        public XPoint? IntersectLines(XLineSegment aSeg,
                                     bool aIgnoreEndpoints = false,
                                     bool aLines = true)//false,true
        {
            var e = EndPoint - StartPoint;
            var f = aSeg.EndPoint - aSeg.StartPoint;
            var ac = aSeg.StartPoint - StartPoint;

            var d = XVector.CrossProduct(f, e);
            var p = XVector.CrossProduct(f, ac);
            var q = XVector.CrossProduct(e, ac);

            if (d == 0)
                return null;//new Point();

            if (!aLines && d > 0 && (q < 0 || q > d || p < 0 || p > d))
                return null;//new Point();

            if (!aLines && d < 0 && (q < d || p < d || p > 0 || q > 0))
                return null;//new Point();

            if (!aLines && aIgnoreEndpoints && (q == 0 || q == d) && (p == 0 || p == d))
                return null;//new Point();

            var ip = new XPoint(aSeg.StartPoint.X + rescale(q, f.X, d),
                 aSeg.StartPoint.Y + rescale(q, f.Y, d));

            return ip;
        }

        double rescale(double aNumerator, double aValue, double aDenominator)
        {
            return (aNumerator * aValue / aDenominator);
        }
    }
}
