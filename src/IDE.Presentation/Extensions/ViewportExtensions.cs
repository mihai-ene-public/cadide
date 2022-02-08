using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using m3 = System.Windows.Media.Media3D;

namespace IDE.Presentation.Extensions
{
    public static class ViewportExtensions
    {
        public static IList<ISelectableItem> FindHitsInRectangle(this Viewport3DX viewport, Rect rectangle)
        {
            const double Tolerance = 1e-10;
            var camera = viewport.Camera as ProjectionCamera;

            if (camera == null)
            {
                throw new InvalidOperationException("No projection camera defined. Cannot find rectangle hits.");
            }

            if (rectangle.Width < Tolerance && rectangle.Height < Tolerance)
            {
                var hitResults = viewport.FindItemHits(rectangle.BottomLeft);
                return hitResults;
            }

            var results = new List<ISelectableItem>();

            viewport.Traverse<MeshNode>((visual, transform) =>
            {
                var geometry = visual.Geometry as MeshGeometry3D;
                if (geometry == null || geometry.Positions == null || geometry.TriangleIndices == null)
                {
                    return;
                }

                var status = false;

                // transform the positions of the mesh to screen coordinates
                var point2Ds = geometry.Positions.Select(p => new m3.Point3D(p.X, p.Y, p.Z)).Select(transform.Transform).Select(viewport.Project).ToArray();

                // evaluate each triangle
                for (var i = 0; i < geometry.TriangleIndices.Count / 3; i++)
                {
                    var triangle = new Triangle(
                                                point2Ds[geometry.TriangleIndices[i * 3]],
                                                point2Ds[geometry.TriangleIndices[(i * 3) + 1]],
                                                point2Ds[geometry.TriangleIndices[(i * 3) + 2]]);
                    status = status
                             || triangle.IsCompletelyInside(rectangle)
                             || triangle.IntersectsWith(rectangle)
                             || triangle.IsRectCompletelyInside(rectangle);

                    if (status)
                    {
                        break;
                    }
                }

                if (status && visual.WrapperSource is Element3D element)
                {
                    var item = element.DataContext as ISelectableItem;
                    if (item != null)
                        results.Add(item);
                }
            });

            return results;
        }

        private static void Traverse<T>(this Viewport3DX viewport, Action<T, m3.Transform3D> action) where T : SceneNode
        {
            foreach (var element in viewport.Items)
            {
                Traverse(element.SceneNode, action);
            }
        }

        private static void Traverse<T>(SceneNode model, Action<T, m3.Transform3D> action) where T : SceneNode
        {
            if (model is T)
            {
                if (model.WrapperSource is Element3D m)
                {
                    action((T)model, m.Transform);
                }
            }
            foreach (var element in model.Items)
            {
                Traverse(element, action);
            }
        }

        public static IList<ISelectableItem> FindItemHits(this Viewport3DX viewport, Point position)
        {
            var hits = viewport.FindHits(position);

            var hitItems = hits.Select(hitItem => (hitItem.ModelHit as Element3D).DataContext as ISelectableItem)
                             .Where(item => item != null)
                             .ToList();

            return hitItems;
        }
    }

    public class LineSegment
    {
        /// <summary>
        /// The first point of the line segment.
        /// </summary>
        private readonly Point p1;

        /// <summary>
        /// The second point of the line segment.
        /// </summary>
        private readonly Point p2;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment"/> class.
        /// </summary>
        /// <param name="p1">The first point of the line segment.</param>
        /// <param name="p2">The second point of the line segment.</param>
        public LineSegment(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        /// <summary>
        /// Gets the first point of the line segment.
        /// </summary>
        /// <value>The point.</value>
        public Point P1
        {
            get
            {
                return this.p1;
            }
        }

        /// <summary>
        /// Gets the second point of the line segment.
        /// </summary>
        /// <value>The point.</value>
        public Point P2
        {
            get
            {
                return this.p2;
            }
        }

        /// <summary>
        /// Checks if there are any intersections of two line segments.
        /// </summary>
        /// <param name="a1">One vertex of line a.</param>
        /// <param name="a2">The other vertex of the line a.</param>
        /// <param name="b1">One vertex of line b.</param>
        /// <param name="b2">The other vertex of the line b.</param>
        /// <returns>
        /// <c>true</c>, if the two lines are crossed. Otherwise, it returns <c>false</c>.
        /// </returns>
        public static bool AreLineSegmentsIntersecting(Point a1, Point a2, Point b1, Point b2)
        {
            if (b1 == b2 || a1 == a2)
            {
                return false;
            }

            if ((((a2.X - a1.X) * (b1.Y - a1.Y)) - ((b1.X - a1.X) * (a2.Y - a1.Y)))
                * (((a2.X - a1.X) * (b2.Y - a1.Y)) - ((b2.X - a1.X) * (a2.Y - a1.Y))) > 0)
            {
                return false;
            }

            if ((((b2.X - b1.X) * (a1.Y - b1.Y)) - ((a1.X - b1.X) * (b2.Y - b1.Y)))
                * (((b2.X - b1.X) * (a2.Y - b1.Y)) - ((a2.X - b1.X) * (b2.Y - b1.Y))) > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Indicates whether the specified line segment intersects with the current line segment.
        /// </summary>
        /// <param name="other">The line segment to check.</param>
        /// <returns>
        /// <c>true</c> if the specified line segment intersects with the current line segment; otherwise <c>false</c>.
        /// </returns>
        public bool IntersectsWith(LineSegment other)
        {
            return AreLineSegmentsIntersecting(this.p1, this.p2, other.p1, other.p2);
        }
    }

    public class Triangle
    {
        /// <summary>
        /// The first point of the triangle.
        /// </summary>
        private readonly Point p1;

        /// <summary>
        /// The second point of the triangle.
        /// </summary>
        private readonly Point p2;

        /// <summary>
        /// The third point of the triangle.
        /// </summary>
        private readonly Point p3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        /// <param name="a">The first point of the triangle.</param>
        /// <param name="b">The second point of the triangle.</param>
        /// <param name="c">The third point of the triangle.</param>
        public Triangle(Point a, Point b, Point c)
        {
            this.p1 = a;
            this.p2 = b;
            this.p3 = c;
        }

        /// <summary>
        /// Gets the first point of the triangle.
        /// </summary>
        /// <value>The point.</value>
        public Point P1
        {
            get
            {
                return this.p1;
            }
        }

        /// <summary>
        /// Gets the second point of the triangle.
        /// </summary>
        /// <value>The point.</value>
        public Point P2
        {
            get
            {
                return this.p2;
            }
        }

        /// <summary>
        /// Gets the third point of the triangle.
        /// </summary>
        /// <value>The point.</value>
        public Point P3
        {
            get
            {
                return this.p3;
            }
        }

        /// <summary>
        /// Checks whether the specified rectangle is completely inside the current triangle.
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <returns>
        /// <c>true</c> if the specified rectangle is inside the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IsCompletelyInside(Rect rect)
        {
            return rect.Contains(this.p2) && rect.Contains(this.p3) && rect.Contains(this.P3);
        }

        /// <summary>
        /// Checks whether the specified rectangle is completely inside the current triangle.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <returns>
        /// <c>true</c> if the specified rectangle is inside the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IsRectCompletelyInside(Rect rect)
        {
            return this.IsPointInside(rect.TopLeft) && this.IsPointInside(rect.TopRight)
                   && this.IsPointInside(rect.BottomLeft) && this.IsPointInside(rect.BottomRight);
        }

        /// <summary>
        /// Checks whether the specified point is inside the triangle. 
        /// </summary>
        /// <param name="p">The point to be checked.</param>
        /// <returns>
        /// <c>true</c> if the specified point is inside the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IsPointInside(Point p)
        {
            // http://stackoverflow.com/questions/2049582/how-to-determine-a-point-in-a-triangle
            var s = (this.p1.Y * this.p3.X) - (this.p1.X * this.p3.Y) + ((this.p3.Y - this.p1.Y) * p.X) + ((this.p1.X - this.p3.X) * p.Y);
            var t = (this.p1.X * this.p2.Y) - (this.p1.Y * this.p2.X) + ((this.p1.Y - this.p2.Y) * p.X) + ((this.p2.X - this.p1.X) * p.Y);

            if ((s < 0) != (t < 0))
            {
                return false;
            }

            var a = (-this.p2.Y * this.p3.X) + (this.p1.Y * (this.p3.X - this.p2.X)) + (this.p1.X * (this.p2.Y - this.p3.Y)) + (this.p2.X * this.p3.Y);
            if (a < 0.0)
            {
                s = -s;
                t = -t;
                a = -a;
            }

            return s > 0 && t > 0 && (s + t) < a;
        }

        /// <summary>
        /// Indicates whether the specified rectangle intersects with the current triangle.
        /// </summary>
        /// <param name="rect">The rectangle to check.</param>
        /// <returns>
        /// <c>true</c> if the specified rectangle intersects with the current triangle; otherwise <c>false</c>.
        /// </returns>
        public bool IntersectsWith(Rect rect)
        {
            return LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.BottomLeft, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.BottomLeft, rect.TopLeft)
                   || LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.TopLeft, rect.TopRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p1, this.p2, rect.TopRight, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.BottomLeft, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.BottomLeft, rect.TopLeft)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.TopLeft, rect.TopRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p2, this.p3, rect.TopRight, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.BottomLeft, rect.BottomRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.BottomLeft, rect.TopLeft)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.TopLeft, rect.TopRight)
                   || LineSegment.AreLineSegmentsIntersecting(this.p3, this.p1, rect.TopRight, rect.BottomRight);
        }
    }

}
