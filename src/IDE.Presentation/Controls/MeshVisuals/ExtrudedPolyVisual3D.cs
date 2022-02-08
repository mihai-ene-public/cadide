using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace IDE.Controls
{
    public class ExtrudedPolyVisual3D : AbstractMeshGeometryModel3D
    {
        public ExtrudedPolyVisual3D()
        {
            Points = new PointCollection();
        }

        //Points
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points", typeof(PointCollection), typeof(ExtrudedPolyVisual3D), new UIPropertyMetadata(new PointCollection(), GeometryChanged));

        public PointCollection Points
        {
            get
            {
                return (PointCollection)GetValue(PointsProperty);
            }
            set
            {
                SetValue(PointsProperty, value);
            }
        }

        //Height
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(ExtrudedPolyVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }
        }

        protected override HelixToolkit.Wpf.SharpDX.Geometry3D Tessellate()
        {
            if (Points == null || Points.Count < 2)
                return null;

            var builder = new MeshBuilder();
            var axisX = new Vector3(1, 0, 0);
            var axisY = new Vector3(0, 1, 0);
            var startPoint = new Vector3(0, 0, 1) * (float)Height;
            var endPoint = new Vector3(0, 0, 0);

            //if (Points.Count % 2 == 1)
            //    Points.Add(Points[0]);

            var vectorPoints = Points.Select(p => new Vector2((float)p.X, (float)p.Y)).ToList();

            //add first point
            if (vectorPoints[0] != vectorPoints[vectorPoints.Count - 1])
                vectorPoints.Add(vectorPoints[0]);

            if (vectorPoints.Count % 2 == 1)
                vectorPoints.Add(vectorPoints[0]);

            builder.AddExtrudedSegments(vectorPoints, axisX, startPoint, endPoint);

            BuilderAddHorizontalFaces(builder, vectorPoints);

            //builder.Normals = null;
            //builder.ComputeNormalsAndTangents(MeshFaces.Default, builder.HasTangents);

            return builder.ToMesh();
        }



        private void BuilderAddHorizontalFaces(MeshBuilder builder, IList<Vector2> points)
        {
            var polygon = new TriangleNet.Geometry.Polygon();
            polygon.AddContour(points.Select(p => new Vertex(p.X, p.Y)));

            var mesher = new GenericMesher();
            var options = new ConstraintOptions();
            var mesh = mesher.Triangulate(polygon, options);

            var p0 = new Vector3(0, 0, 1) * (float)Height;
            var p1 = new Vector3(0, 0, 0);

            var u = Vector3.UnitX;
            //u.Normalize();
            var z = p1 - p0;
            var h = z.Length();
            z.Normalize();
            var v = Vector3.Cross(z, u);

            // Convert the triangles
            foreach (var t in mesh.Triangles)
            {
                var v0 = t.GetVertex(2);
                var v1 = t.GetVertex(1);
                var v2 = t.GetVertex(0);

                // Add the top triangle.
                // Project the X/Y vertices onto a plane defined by textdirection, p0 and p1.                
                builder.AddTriangle(Project(v0, p0, u, v, z, h), Project(v1, p0, u, v, z, h), Project(v2, p0, u, v, z, h));

                // Add the bottom triangle.
                builder.AddTriangle(Project(v2, p0, u, v, z, 0), Project(v1, p0, u, v, z, 0), Project(v0, p0, u, v, z, 0));
            }
        }

        private Vector3 Project(Vertex v, Vector3 p0, Vector3 x, Vector3 y, Vector3 z, double h)
        {
            return (p0 + x * (float)v.X - y * (float)v.Y + z * (float)h);
        }

    }
}
