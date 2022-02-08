using System.ComponentModel;
using System.Collections.ObjectModel;
using IDE.Core.Types.Media3D;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public abstract class ParametricPackageMeshItem : BaseMeshItem
    {
        public ParametricPackageMeshItem()
        {

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
        public bool HasPreview => true;

        [Browsable(false)]
        public new XColor FillColor { get; set; }

        [Browsable(false)]
        public new int PadNumber { get; set; }

        protected bool isLoading = false;

        private IList<BaseMeshItem> items = new ObservableCollection<BaseMeshItem>();

        [Browsable(false)]
        public IList<BaseMeshItem> Items
        {
            get { return items; }
            set
            {
                items = new ObservableCollection<BaseMeshItem>(value);
                OnPropertyChanged(nameof(Items));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 1000)]
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
        [Display(Order = 1001)]
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
        [Display(Order = 1002)]
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
        [Display(Order = 1003,Name ="Rotation X")]
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
        [Display(Order = 1004, Name = "Rotation Y")]
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
        [Display(Order = 1005, Name = "Rotation Z")]
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

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = primitive as ParametricPackageBase3DItem;

            X = r.CenterX;
            Y = r.CenterY;
            Z = r.CenterZ;
            RotationX = r.RotationX;
            RotationY = r.RotationY;
            RotationZ = r.RotationZ;
        }

        protected void SaveToPrimitiveBase(IPrimitive primitive)
        {
            var r = primitive as ParametricPackageBase3DItem;

            r.CenterX = X;
            r.CenterY = Y;
            r.CenterZ = Z;
            r.RotationX = RotationX;
            r.RotationY = RotationY;
            r.RotationZ = RotationZ;
        }

        public override void Translate(double dx, double dy, double dz)
        {
            X += dx;
            Y += dy;
            Z += dz;
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
            RotationZ = RotateSafe(rotationZ);
        }

        public abstract Task GenerateItems();

    }
}
