using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;

namespace IDE.Core.Presentation.Utilities
{
    public static class PolylineExtensions
    {
        public static XLineSegment GetSegment(this ISegmentedPolylineSelectableCanvasItem item, int index)
        {
            var points = item.Points;
            return points.GetSegment(index);
        }

        public static XLineSegment GetSegment(this IList<XPoint> points, int index)
        {
            if (index >= 0 && index < points.Count - 1)
            {
                return new XLineSegment(points[index], points[index + 1]);
            }

            return null;
        }

        public static List<XLineSegment> GetSegments(this ISegmentedPolylineSelectableCanvasItem item)
        {
            var points = item.Points;
            return points.GetSegments();
        }

        public static List<XLineSegment> GetSegments(this IList<XPoint> points)
        {
            var segments = new List<XLineSegment>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                var segment = new XLineSegment(points[i], points[i + 1]);
                segments.Add(segment);
            }

            return segments;
        }

        public static int GetSegmentAtMousePosition(this ISegmentedPolylineSelectableCanvasItem item, XPoint mousePositionMM)
        {
            var segments = item.GetSegments();

            for (int i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                if (seg.IsPointOnLine2(mousePositionMM, item.Width))
                {
                    return i;
                }
            }

            return -1;
        }

        public static IList<int> GetIntersectedSegmentsWith(this ISegmentedPolylineSelectableCanvasItem item, XRect rect)
        {
            var segmentIndexes = new List<int>();

            var segments = item.GetSegments();

            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i].Intersects(rect))
                    segmentIndexes.Add(i);
            }

            return segmentIndexes;
        }

        //removes duplicate vertices and colinear adjacent segments
        public static IList<XPoint> Simplify(this IList<XPoint> points)
        {
            var simplifiedPoints = new List<XPoint>();
            var simplified = SimplifyPolyline(points, simplifiedPoints);
            if (simplified)
            {
                return simplifiedPoints;
            }

            return points;
        }

        /*
         * * Replaces points with indices in range [start_index, end_index] with the points from
            * line chain aLine.
            * @param aStartIndex start of the point range to be replaced (inclusive)
            * @param aEndIndex end of the point range to be replaced (inclusive)
            * @param aLine replacement line chain.
        */
        public static void Replace(this IList<XPoint> points, int startIndex, int endIndex, IList<XPoint> replacementLine)
        {
            if (replacementLine == null || replacementLine.Count == 0)
                return;

            if (endIndex < 0)
                endIndex += points.Count;

            if (startIndex < 0)
                startIndex += points.Count;

            var toRemove = new List<XPoint>();
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                toRemove.Add(points[i]);
            }
            foreach (var p in toRemove)
                points.Remove(p);

            for (int i = 0; i < replacementLine.Count; i++)
            {
                points.Insert(startIndex + i, replacementLine[i]);
            }

        }

        private static bool SimplifyPolyline(IList<XPoint> linePoints, IList<XPoint> simplifiedPoints)
        {
            if (linePoints.Count <= 2)
                return false;

            var uniquePoints = new List<XPoint>();
            int i = 0;
            int np = linePoints.Count;

            // stage 1: eliminate duplicate vertices
            while (i < np)
            {
                int j = i + 1;

                while (j < np && linePoints[i] == linePoints[j])
                    j++;

                uniquePoints.Add(linePoints[i]);
                i = j;
            }

            np = uniquePoints.Count;

            i = 0;

            // stage 2: eliminate collinear segments
            while (i < np - 2)
            {
                var p0 = uniquePoints[i];
                var p1 = uniquePoints[i + 1];
                //var p2 = pts_unique[i + 2];
                int n = i;

                while (n < np - 2 && ArePointsColinear(p0, p1, uniquePoints[n + 2]))
                    n++;

                simplifiedPoints.Add(p0);

                if (n > i)
                    i = n;

                if (n == np)
                {
                    simplifiedPoints.Add(uniquePoints[n - 1]);
                    return true;
                }

                i++;
            }

            if (np > 1)
                simplifiedPoints.Add(uniquePoints[np - 2]);

            simplifiedPoints.Add(uniquePoints[np - 1]);

            return simplifiedPoints.Count < linePoints.Count;//simplified;
        }

        private static bool ArePointsColinear(XPoint p0, XPoint p1, XPoint p2)
        {
            var r = p1 - p0;
            var s = p2 - p1;
            var rxs = Math.Abs(XVector.CrossProduct(r, s));

            var eps = 1e-3;
            return (rxs <= eps);// && qpxr <= eps);
        }
    }
}
