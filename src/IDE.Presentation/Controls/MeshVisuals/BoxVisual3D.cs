using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace IDE.Controls
{
    public class BoxVisual3D : AbstractMeshGeometryModel3D
    {
        /// <summary>
        /// Identifies the <see cref="BottomFace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomFaceProperty = DependencyProperty.Register(
            "BottomFace", typeof(bool), typeof(BoxVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(BoxVisual3D), new UIPropertyMetadata(new Point3D(), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Height"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(BoxVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Length"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
            "Length", typeof(double), typeof(BoxVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="TopFace"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopFaceProperty = DependencyProperty.Register(
            "TopFace", typeof(bool), typeof(BoxVisual3D), new UIPropertyMetadata(true, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Width"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(BoxVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        /// <summary>
        /// Gets or sets a value indicating whether to include the bottom face.
        /// </summary>
        public bool BottomFace
        {
            get
            {
                return (bool)this.GetValue(BottomFaceProperty);
            }

            set
            {
                this.SetValue(BottomFaceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the center of the box.
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
        /// Gets or sets the height (along local z-axis).
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }

            set
            {
                this.SetValue(HeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the length of the box (along local x-axis).
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
        /// Gets or sets a value indicating whether to include the top face.
        /// </summary>
        public bool TopFace
        {
            get
            {
                return (bool)this.GetValue(TopFaceProperty);
            }

            set
            {
                this.SetValue(TopFaceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the box (along local y-axis).
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

        /// <summary>
        /// Do the tessellation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns>The mesh geometry.</returns>
        protected override HelixToolkit.Wpf.SharpDX.Geometry3D Tessellate()
        {
            var center = Center;
            var centerPos = new Vector3((float)center.X, (float)center.Y, (float)center.Z);

            var b = new MeshBuilder();
            b.AddCubeFace(centerPos, new Vector3(-1, 0, 0), new Vector3(0, 0, 1), Length, Width, Height);
            b.AddCubeFace(centerPos, new Vector3(1, 0, 0), new Vector3(0, 0, -1), Length, Width, Height);
            b.AddCubeFace(centerPos, new Vector3(0, -1, 0), new Vector3(0, 0, 1), Width, Length, Height);
            b.AddCubeFace(centerPos, new Vector3(0, 1, 0), new Vector3(0, 0, -1), Width, Length, Height);
            if (this.TopFace)
            {
                b.AddCubeFace(centerPos, new Vector3(0, 0, 1), new Vector3(0, -1, 0), Height, Length, Width);
            }

            if (this.BottomFace)
            {
                b.AddCubeFace(centerPos, new Vector3(0, 0, -1), new Vector3(0, 1, 0), Height, Length, Width);
            }

            return b.ToMesh();
        }
    }
}
