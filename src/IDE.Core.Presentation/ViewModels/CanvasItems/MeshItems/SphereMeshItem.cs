using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class SphereMeshItem : BaseMeshItem
    {
        public SphereMeshItem()
        {
            Radius = 1;
            ThetaDivisions = 60;
            PhiDivisions = 30;
            FillColor = XColor.FromHexString("#FF000080");

        }

        double radius;

        [Display(Order = 2)]
        [MarksDirty]
        public double Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                OnPropertyChanged(nameof(Radius));
            }
        }



        int thetaDivisions;

        [Display(Order = 3)]
        [MarksDirty]
        public int ThetaDivisions
        {
            get { return thetaDivisions; }
            set
            {
                thetaDivisions = value;
                OnPropertyChanged(nameof(ThetaDivisions));
            }
        }

        int phyDivisions;
        [Display(Order = 4)]
        [MarksDirty]
        public int PhiDivisions
        {
            get { return phyDivisions; }
            set
            {
                phyDivisions = value;
                OnPropertyChanged(nameof(PhiDivisions));
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



        public override void Translate(double dx, double dy, double dz)
        {
            X += dx;
            Y += dy;
            Z += dz;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (Sphere3DItem)primitive;

            X = r.CenterX;
            Y = r.CenterY;
            Z = r.CenterZ;
            Radius = r.Radius;
            ThetaDivisions = r.ThetaDivisions;
            PhiDivisions = r.PhiDivisions;
            FillColor = XColor.FromHexString(r.FillColor);
            PadNumber = r.PadNumber;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new Sphere3DItem();

            r.CenterX = X;
            r.CenterY = Y;
            r.CenterZ = Z;
            r.Radius = Radius;
            r.ThetaDivisions = ThetaDivisions;
            r.PhiDivisions = PhiDivisions;
            r.FillColor = FillColor.ToHexString();
            r.PadNumber = PadNumber;

            return r;
        }

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }
        public override void TransformBy(XMatrix3D matrix)
        {
            var p = new XPoint3D(x, y, z);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }
        public override void Rotate(double angle = 90)
        {
        }

        public override string ToString()
        {
            return $"Sphere ({PadNumber})";
        }
    }
}
