using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace IDE.Core
{
    public static class GeometryExtensions
    {
        public static Size ToSize(this XSize size)
        {
            return new Size(size.Width, size.Height);
        }

        public static XRect ToXRect(this Rect rect)
        {
            if (rect.IsEmpty)
                return XRect.Empty;
            return new XRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rect ToRect(this XRect rect)
        {
            if (rect.IsEmpty)
                return Rect.Empty;
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Color ToColor(this XColor c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static XColor ToXColor(this Color c)
        {
            return XColor.FromArgb(c.A, c.R, c.G, c.B);
        }

        public static Transform ToMatrixTransform(this XTransform t)
        {
            var m = t.Value;
            return new MatrixTransform(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        public static Point ToPoint(this XPoint p)
        {
            return new Point(p.X, p.Y);
        }

        public static XPoint ToXPoint(this Point p)
        {
            return new XPoint(p.X, p.Y);
        }

        public static SharpDX.Vector2 ToVector2(this XPoint p)
        {
            return new SharpDX.Vector2((float)p.X, (float)p.Y);
        }

        public static SharpDX.Vector2 ToVector2(this Point p)
        {
            return new SharpDX.Vector2((float)p.X, (float)p.Y);
        }

        public static SharpDX.Vector3 ToVector3(this XPoint3D p)
        {
            return new SharpDX.Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static SharpDX.Vector3 ToVector3(this Point3D p)
        {
            return new SharpDX.Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static SharpDX.Vector3 ToVector3(this XVector3D p)
        {
            return new SharpDX.Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }
        public static SharpDX.Vector3 ToVector3(this Vector3D p)
        {
            return new SharpDX.Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }

        public static SweepDirection ToSweepDirection(this XSweepDirection sd)
        {
            return (SweepDirection)sd;
        }

    }
}
