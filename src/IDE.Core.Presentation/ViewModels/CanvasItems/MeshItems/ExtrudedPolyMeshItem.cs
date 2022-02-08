using IDE.Core.Storage;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class ExtrudedPolyMeshItem : BaseMeshItem
    {
        public ExtrudedPolyMeshItem()
        {
            Height = 1;
            FillColor = XColor.FromHexString("#FF000080");
            points = new ObservableCollection<XPoint>();
        }

        ObservableCollection<XPoint> points;

        [Browsable(false)]
        public ObservableCollection<XPoint> Points
        {
            get { return points; }
            //set
            //{
            //    points = value;
            //    OnPropertyChanged(nameof(Points));
            //}
        }

        double height;
        [Display(Order = 4)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Height
        {
            get { return height; }
            set
            {
                if (height == value) return;
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 5)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Center));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 6)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Center));
            }
        }

        double z;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 7)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged(nameof(Z));
                OnPropertyChanged(nameof(PositionZ));
                OnPropertyChanged(nameof(Center));
            }
        }

        [Browsable(false)]
        public double PositionZ
        {
            get { return -Z; }
        }

        [Browsable(false)]
        public XPoint3D Center
        {
            get
            {
                return new XPoint3D(X, Y, Z);
            }
        }



        double rotationX;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 8)]
        [MarksDirty]
        public double RotationX
        {
            get { return rotationX; }
            set
            {
                rotationX = value;
                OnPropertyChanged(nameof(RotationX));
            }
        }

        double rotationY;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 9)]
        [MarksDirty]
        public double RotationY
        {
            get { return rotationY; }
            set
            {
                rotationY = value;
                OnPropertyChanged(nameof(RotationY));
            }
        }

        double rotationZ;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 10)]
        [MarksDirty]
        public double RotationZ
        {
            get { return rotationZ; }
            set
            {
                rotationZ = value;
                OnPropertyChanged(nameof(RotationZ));
            }
        }



        public override void Translate(double dx, double dy, double dz)
        {
            X += dx;
            Y += dy;
            Z += dz;
        }

        const double minWidth = 2;
        const double minHeight = 2;


        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (ExtrudedPoly3DItem)primitive;

            X = r.CenterX;
            Y = r.CenterY;
            Z = r.CenterZ;
            Height = r.Height;
            RotationX = r.RotationX;
            RotationY = r.RotationY;
            RotationZ = r.RotationZ;
            FillColor = XColor.FromHexString(r.FillColor);
            points.Clear();
            points.AddRange(r.Points);
            PadNumber = r.PadNumber;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new ExtrudedPoly3DItem();

            r.CenterX = X;
            r.CenterY = Y;
            r.CenterZ = Z;
            r.Height = Height;
            r.RotationX = RotationX;
            r.RotationY = RotationY;
            r.RotationZ = RotationZ;
            r.FillColor = FillColor.ToHexString();
            r.Points = Points.ToList();
            r.PadNumber = PadNumber;

            return r;
        }

        public override void MirrorX()
        {
            var r = (rotationX + 180) % 360;
            RotationX = r;
        }

        public override void MirrorY()
        {
            var r = (rotationY + 180) % 360;
            RotationY = r;
        }

        public override void TransformBy(XMatrix3D matrix)
        {
            var p = new XPoint3D(x, y, z);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;
            Z = p.Z;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            RotationZ = RotateSafe(rotationZ, rotAngle);

        }

        public override void Rotate()
        {
            //var r = (rotationZ + 90) % 360;
            RotationZ = RotateSafe(rotationZ);
        }

        public override string ToString()
        {
            return $"Poly ({PadNumber})";
        }
    }
}
