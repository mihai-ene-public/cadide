using System;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using IDE.Presentation.Extensions;

namespace IDE.Controls
{
    public class GridLinesVisual3D : LineGeometryModel3D
    {
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(GridLinesVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="MinorDistance"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinorDistanceProperty = DependencyProperty.Register(
            "MinorDistance", typeof(double), typeof(GridLinesVisual3D), new PropertyMetadata(2.5, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="LengthDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthDirectionProperty =
            DependencyProperty.Register(
                "LengthDirection",
                typeof(Vector3D),
                typeof(GridLinesVisual3D),
                new UIPropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(GridLinesVisual3D), new PropertyMetadata(200.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="MajorDistance"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MajorDistanceProperty = DependencyProperty.Register(
            "MajorDistance", typeof(double), typeof(GridLinesVisual3D), new PropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Normal"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NormalProperty = DependencyProperty.Register(
            "Normal",
            typeof(Vector3D),
            typeof(GridLinesVisual3D),
            new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));


        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(GridLinesVisual3D), new PropertyMetadata(200.0, GeometryChanged));

        /// <summary>
        /// The length direction.
        /// </summary>
        private Vector3D lengthDirection;

        /// <summary>
        /// The width direction.
        /// </summary>
        private Vector3D widthDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref = "GridLinesVisual3D" /> class.
        /// </summary>
        public GridLinesVisual3D()
        {
            this.Color = System.Windows.Media.Colors.Gray;
        }

        private static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GridLinesVisual3D)d).OnGeometryChanged();
        }

        private void OnGeometryChanged()
        {
            Geometry = this.Visible ? this.Tessellate() : null;
        }

        /// <summary>
        /// Gets or sets the center of the grid.
        /// </summary>
        /// <value>The center.</value>
        public Point3D Center
        {
            get
            {
                return (Point3D)this.GetValue(CenterProperty);
            }

            set
            {
                this.SetValue(CenterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the grid area.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get
            {
                return (double)this.GetValue(LengthProperty);
            }

            set
            {
                this.SetValue(LengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length direction of the grid.
        /// </summary>
        /// <value>The length direction.</value>
        public Vector3D LengthDirection
        {
            get
            {
                return (Vector3D)this.GetValue(LengthDirectionProperty);
            }

            set
            {
                this.SetValue(LengthDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between major grid lines.
        /// </summary>
        /// <value>The distance.</value>
        public double MajorDistance
        {
            get
            {
                return (double)this.GetValue(MajorDistanceProperty);
            }

            set
            {
                this.SetValue(MajorDistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance between minor grid lines.
        /// </summary>
        /// <value>The distance.</value>
        public double MinorDistance
        {
            get
            {
                return (double)this.GetValue(MinorDistanceProperty);
            }

            set
            {
                this.SetValue(MinorDistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the normal vector of the grid plane.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get
            {
                return (Vector3D)this.GetValue(NormalProperty);
            }

            set
            {
                this.SetValue(NormalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the grid area (perpendicular to the length direction).
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get
            {
                return (double)this.GetValue(WidthProperty);
            }

            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        protected HelixToolkit.Wpf.SharpDX.Geometry3D Tessellate()
        {
            this.lengthDirection = this.LengthDirection;
            this.lengthDirection.Normalize();

            // #136, chrkon, 2015-03-26
            // if NormalVector and LenghtDirection are not perpendicular then overwrite LengthDirection
            if (Vector3D.DotProduct(this.Normal, this.LengthDirection) != 0.0)
            {
                this.lengthDirection = this.Normal.FindAnyPerpendicular();
                this.lengthDirection.Normalize();
            }

            // create WidthDirection by rotating lengthDirection vector 90° around normal vector
            var rotate = new RotateTransform3D(new AxisAngleRotation3D(this.Normal, 90.0));
            this.widthDirection = rotate.Transform(this.lengthDirection);
            this.widthDirection.Normalize();
            // #136 

            var gridMinor = new LineBuilder();
            //var gridMajor = new LineBuilder();
            double minX = -this.Width / 2;
            double minY = -this.Length / 2;
            double maxX = this.Width / 2;
            double maxY = this.Length / 2;

            double x = minX;
            double eps = this.MinorDistance / 10;
            while (x <= maxX + eps)
            {
                double t = this.Thickness;
                //if (IsMultipleOf(x, this.MajorDistance))
                //{
                //    t *= 2;
                //}

                this.AddLineX(gridMinor, x, minY, maxY);
                x += this.MinorDistance;
            }

            double y = minY;
            while (y <= maxY + eps)
            {
                double t = this.Thickness;
                //if (IsMultipleOf(y, this.MajorDistance))
                //{
                //    t *= 2;
                //}

                this.AddLineY(gridMinor, y, minX, maxX);
                y += this.MinorDistance;
            }

            //var m = mesh.ToMesh();
            //m.Freeze();
            var m = gridMinor.ToLineGeometry3D();
            return m;
        }

        private static bool IsMultipleOf(double y, double d)
        {
            double y2 = d * (int)(y / d);
            return Math.Abs(y - y2) < 1e-3;
        }

        private void AddLineX(LineBuilder mesh, double x, double minY, double maxY)
        {
            var p1 = GetPoint(x, minY);
            var p2 = GetPoint(x, maxY);
            mesh.AddLine(new SharpDX.Vector3((float)p1.X, (float)p1.Y, (float)p1.Z) , new SharpDX.Vector3((float)p2.X, (float)p2.Y, (float)p2.Z));
        }

        private void AddLineY(LineBuilder mesh, double y, double minX, double maxX)
        {
            var p1 = GetPoint(minX, y);
            var p2 = GetPoint(maxX, y);
            mesh.AddLine(new SharpDX.Vector3((float)p1.X, (float)p1.Y, (float)p1.Z), new SharpDX.Vector3((float)p2.X, (float)p2.Y, (float)p2.Z));
        }

        private Point3D GetPoint(double x, double y)
        {
            return this.Center + (this.widthDirection * x) + (this.lengthDirection * y);
        }
    }
}
