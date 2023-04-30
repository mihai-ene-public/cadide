using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;

namespace IDE.Core.Designers
{
    public class BoxMeshItem : BaseMeshItem
    {

        public BoxMeshItem()
        {
            Width = 1;
            Height = 1;
            Length = 1;
            FillColor = XColor.FromHexString("#FF000080");
        }

        double width;
        [Display(Order = 2)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Width
        {
            get { return width; }
            set
            {
                if (width == value) return;
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        double length;
        [Display(Order = 3)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Length
        {
            get { return length; }
            set
            {
                if (length == value) return;
                length = value;
                OnPropertyChanged(nameof(Length));
            }
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
                OnPropertyChanged(nameof(Center));
            }
        }

        [Browsable(false)]
        public XPoint3D Center
        {
            get
            {
                return new XPoint3D(X, Y, -Z);
            }
        }

        private bool topFace = true;

        [Browsable(false)]
        public bool TopFace
        {
            get { return topFace; }
            set
            {
                topFace = value;
                OnPropertyChanged(nameof(TopFace));
            }
        }

        private bool bottomFace = true;
        
        [Browsable(false)]
        public bool BottomFace
        {
            get { return bottomFace; }
            set
            {
                bottomFace = value;
                OnPropertyChanged(nameof(BottomFace));
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
            var r = (Box3DItem)primitive;

            X = r.CenterX;
            Y = r.CenterY;
            Z = r.CenterZ;
            Width = r.Width;
            Height = r.Height;
            Length = r.Length;
            RotationX = r.RotationX;
            RotationY = r.RotationY;
            RotationZ = r.RotationZ;
            FillColor = XColor.FromHexString(r.FillColor);
            PadNumber = r.PadNumber;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new Box3DItem();

            r.CenterX = X;
            r.CenterY = Y;
            r.CenterZ = Z;
            r.Width = Width;
            r.Height = Height;
            r.Length = Length;
            r.RotationX = RotationX;
            r.RotationY = RotationY;
            r.RotationZ = RotationZ;
            r.FillColor = FillColor.ToHexString();
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

        public override void Rotate(double angle = 90)
        {
            //var r = (rotationZ + 90) % 360;
            RotationZ = RotateSafe(rotationZ, angle);
        }

        public override string ToString()
        {
            return $"Box ({PadNumber})";
        }
    }
}
