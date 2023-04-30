using IDE.Core.Storage;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using IDE.Core.Common;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class GroupMeshItem : BaseMeshItem
    {
        public GroupMeshItem()
        {
            Items = new ObservableCollection<BaseMeshItem>();

            PropertyChanged += (s, e) =>
              {
                  if (e.PropertyName == nameof(IsSelected))
                  {
                      foreach (var item in Items)
                          item.IsSelected = IsSelected;
                  }
              };
        }

        [Browsable(false)]
        public ObservableCollection<BaseMeshItem> Items { get; set; }

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

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (Group3DItem)primitive;

            X = r.CenterX;
            Y = r.CenterY;
            Z = r.CenterZ;
            RotationX = r.RotationX;
            RotationY = r.RotationY;
            RotationZ = r.RotationZ;
            PadNumber = r.PadNumber;
            FillColor = XColor.FromHexString(r.FillColor);

            if (r.Items != null)
                Items.AddRange(r.Items.Select(p => (BaseMeshItem)p.CreateDesignerItem()));
            foreach (var p in Items)
                p.ParentObject = this;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new Group3DItem();

            r.CenterX = X;
            r.CenterY = Y;
            r.CenterZ = Z;
            r.RotationX = RotationX;
            r.RotationY = RotationY;
            r.RotationZ = RotationZ;
            r.FillColor = FillColor.ToHexString();
            r.PadNumber = PadNumber;
            r.Items= Items.Cast<BaseMeshItem>().Select(d => (MeshPrimitive)d.SaveToPrimitive()).ToList();

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
            // var r = (rotationZ + 90) % 360;
            RotationZ = RotateSafe(rotationZ, angle);
        }

        public override string ToString()
        {
            return $"Group ({PadNumber})";
        }
    }

}
