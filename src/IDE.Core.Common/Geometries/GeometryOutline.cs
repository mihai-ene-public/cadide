using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ClipperLib;
using System.Linq;
using IDE.Core.Interfaces.Geometries;

namespace IDE.Core.Common.Geometries
{

    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;


    public abstract class GeometryOutline : IGeometryOutline
    {
        public const int DefaultNumberOfSegmentsPerCircle = 24;//40;


        const int DefaultClipperMultiplyValue = 4000000;//100000;

        protected List<XPoint> points = new List<XPoint>();

        public XTransform Transform { get; set; }

        /// <summary>
        /// returns the points outline with applied transform
        /// </summary>
        /// <returns></returns>
        public List<XPoint> GetOutline()
        {
            if (Transform == null)
                return points;

            var transformedOutline = new List<XPoint>();

            for (int i = 0; i < points.Count; i++)
                transformedOutline.Add(Transform.Transform(points[i]));

            return transformedOutline;
        }

        public List<XPoint> ApplyTransform(XTransform transform)
        {
            Transform = transform;

            return GetOutline();
        }

        protected IList<XPoint> GetCirclePoints(XPoint offset,
                                                int thetaDivisions,
                                                double angleStart = 0,
                                                double totalAngle = 2 * Math.PI,
                                                double radius = 1.0,
                                                bool counterClockwise = true)
        {
            var points = GetEllipsePoints(
                                          offset,
                                          thetaDivisions,
                                          angleStart,
                                          totalAngle,
                                          radius,
                                          radius,
                                          counterClockwise
                                         );

            return points;
        }

        protected IList<XPoint> GetEllipsePoints(XPoint offset,
                                                int thetaDivisions,
                                                double angleStart = 0,
                                                double totalAngle = 2 * Math.PI,
                                                double radiusX = 1.0,
                                                double radiusY = 1.0,
                                                bool counterClockwise = true)
        {
            var points = new List<XPoint>();
            var deltaTheta = totalAngle / thetaDivisions;
            int direction = counterClockwise ? 1 : -1;

            var theta = angleStart;

            for (int i = 0; i <= thetaDivisions; i++)
            {
                points.Add(new XPoint(offset.X + radiusX * Math.Cos(theta), offset.Y - radiusY * Math.Sin(theta)));

                theta += direction * deltaTheta;
            }

            return points;
        }

        protected IList<XPoint> GetEllipsePoints(XPoint startPoint,
                                                 XPoint endPoint,
                                                 double radiusX,
                                                 double radiusY,
                                                 XSweepDirection sweepDirection,
                                                  int thetaDivisions
                                                )
        {
            var center = GeometryTools.GetArcCenter(startPoint, endPoint, radiusX, radiusY, sweepDirection);
            var zeroVector = new XVector(1, 0);
            var spVector = startPoint - center;
            spVector.Normalize();

            var epVector = endPoint - center;
            epVector.Normalize();

            var angleStart = XVector.AngleBetweenRadians(spVector, zeroVector);
            var angleEnd = XVector.AngleBetweenRadians(epVector, zeroVector);

            //when CW, the angle should shrink
            if (sweepDirection == XSweepDirection.Clockwise && angleStart < angleEnd)
            {
                if (angleStart < 0)
                    angleStart += 2 * Math.PI;
            }
            var totalAngle = Math.Abs(angleEnd - angleStart);

            return GetEllipsePoints(center, thetaDivisions, angleStart, totalAngle, radiusX, radiusY, sweepDirection == XSweepDirection.Counterclockwise);
        }

        protected static IntPoint ToIntPoint(XPoint point, int toleranceMultiplier = DefaultClipperMultiplyValue)
        {
            return new IntPoint
            {
                X = (long)(point.X * toleranceMultiplier),
                Y = (long)(point.Y * toleranceMultiplier)
            };
        }

        protected static long ToIntValue(double value, int toleranceMultiplier = DefaultClipperMultiplyValue)
        {
            return (long)(value * toleranceMultiplier);
        }

        protected static XPoint FromIntPoint(IntPoint point, int scalingFactor = DefaultClipperMultiplyValue)
        {
            return new XPoint
            {
                X = point.X / (double)scalingFactor,
                Y = point.Y / (double)scalingFactor
            };
        }

        public IList<XPoint> GetOutlinedGeometry(IList<XPoint> polyline, double offset)
        {
            var clipper = new ClipperOffset(arcTolerance: ToIntValue(0.005 * offset));

            var path = new Path();
            var solution = new Paths();
            path.AddRange(polyline.Select(p => ToIntPoint(p)));

            clipper.AddPath(path, JoinType.jtRound, EndType.etOpenRound);

            offset = ToIntValue(offset);

            clipper.Execute(ref solution, offset);

            if (solution.Count > 0)
            {
                var resultPath = solution[0];

                return resultPath.Select(p => FromIntPoint(p)).ToList();
            }

            return new List<XPoint>();
        }

        private static ClipType ClipTypeFromGeometryCombineMode(GeometryCombineMode mode)
        {
            var ct = mode switch
            {
                GeometryCombineMode.Union => ClipType.ctUnion,
                GeometryCombineMode.Intersect => ClipType.ctIntersection,
                GeometryCombineMode.Exclude => ClipType.ctDifference,
                GeometryCombineMode.Xor => ClipType.ctXor,
                _ => ClipType.ctUnion
            };

            return ct;
        }

        private static Path GetPathFromOutline(IList<XPoint> outline)
        {
            var path = new Path();
            path.AddRange(outline.Select(p => ToIntPoint(p)));

            return path;
        }

        public static IList<IList<XPoint>> Combine(IList<IList<XPoint>> geometry1, IList<IList<XPoint>> geometry2, GeometryCombineMode mode)
        {
            var clipper = new Clipper();
            //clipper.StrictlySimple = true;


            //outlines from geometry1
            foreach (var outline1 in geometry1)
            {
                var path1 = new Path();
                path1.AddRange(outline1.Select(p => ToIntPoint(p)));

                var area = Clipper.Area(path1);

                clipper.AddPath(path1, PolyType.ptSubject, true);
            }


            //outlines from geometry2
            if (geometry2 != null)
            {
                foreach (var outline2 in geometry2)
                {
                    var path2 = new Path();
                    path2.AddRange(outline2.Select(p => ToIntPoint(p)));
                    var area = Clipper.Area(path2);
                    if (area < 0)
                        path2.Reverse();

                    clipper.AddPath(path2, PolyType.ptClip, true);
                }
            }

            var clipType = ClipTypeFromGeometryCombineMode(mode);

            IList<IList<XPoint>> outlines = new List<IList<XPoint>>();

            var solution = new Paths();
            clipper.Execute(clipType, solution, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
            for (int i = 0; i < solution.Count; i++)
            {
                var outline = solution[i].Select(p => FromIntPoint(p)).ToList();
                outlines.Add(outline);
            }

            return outlines;
        }




        public static IGeometryOutline Combine(IList<IGeometryOutline> subjectGeometries, IList<IGeometryOutline> clipGeometries, GeometryCombineMode mode)
        {
            var geometry1 = new List<IList<XPoint>>();
            var geometry2 = new List<IList<XPoint>>();

            foreach (var g in subjectGeometries)
            {
                LoadPaths(g, geometry1);
            }

            if (clipGeometries != null)
            {
                foreach (var g in clipGeometries)
                {
                    LoadPaths(g, geometry2);
                }
            }

            var outlines = Combine(geometry1, geometry2, mode);

            var geometry = new GeometryOutlines();
            foreach (var outline in outlines)
            {
                var poly = new PolygonGeometryOutline(outline);
                geometry.Outlines.Add(poly);
            }

            return geometry;
        }

        protected static void LoadPaths(IGeometryOutline geometry, IList<IList<XPoint>> paths)
        {
            if (geometry is GeometryOutlines go)
            {
                foreach (var g in go.Outlines)
                {
                    LoadPaths(g, paths);
                }
            }
            else
            {
                var outline = geometry.GetOutline();
                paths.Add(outline);
            }
        }

        public static GeometryOutlineOrientation GetOutlineOrientation(IList<XPoint> outline)
        {
            var path = GetPathFromOutline(outline);

            var orientation = GeometryOutlineOrientation.Positive;
            var pathOrientation = Clipper.Orientation(path);

            if (!pathOrientation)
                orientation = GeometryOutlineOrientation.Negative;

            return orientation;
        }
    }

    public enum GeometryCombineMode
    {
        //
        // Summary:
        //     The two regions are combined by taking the union of both. The resulting geometry
        //     is geometry A + geometry B.
        Union = 0,
        //
        // Summary:
        //     The two regions are combined by taking their intersection. The new area consists
        //     of the overlapping region between the two geometries.
        Intersect = 1,
        //
        // Summary:
        //     The two regions are combined by taking the area that exists in the first region
        //     but not the second and the area that exists in the second region but not the
        //     first. The new region consists of (A-B) + (B-A), where A and B are geometries.
        Xor = 2,
        //
        // Summary:
        //     The second region is excluded from the first. Given two geometries, A and B,
        //     the area of geometry B is removed from the area of geometry A, producing a region
        //     that is A-B.
        Exclude = 3
    }

    public enum GeometryOutlineOrientation
    {
        Negative = 0,
        Positive = 1
    }
}
