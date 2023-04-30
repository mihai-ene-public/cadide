using IDE.Core.Interfaces;
using IDE.Core.Types.Media3D;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    /// <summary>
    /// a temporary group created for placement. Used for import similar items or after paste
    /// <para>This is not saved in document</para>
    /// </summary>
    public class VolatileGroup3DCanvasItem : BaseMeshItem
    {
        public VolatileGroup3DCanvasItem()
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


        List<ISelectableItem> items = new List<ISelectableItem>();

        //it will need some items that are not selectable
        [Browsable(false)]
        public List<ISelectableItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 5)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
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

        public override void Rotate(double angle = 90)
        {
            // var r = (rotationZ + 90) % 360;
            RotationZ = RotateSafe(rotationZ, angle);
        }
    }
}
