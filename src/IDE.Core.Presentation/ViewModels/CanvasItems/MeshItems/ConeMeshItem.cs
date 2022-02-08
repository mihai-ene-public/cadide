using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;

namespace IDE.Core.Designers
{
    public class ConeMeshItem : BaseMeshItem
    {
        public ConeMeshItem()
        {
            DirectionX = 0;
            DirectionY = 0;
            DirectionZ = 1;
            ShowBaseCap = true;
            ShowTopCap = true;
            BaseRadius = 1;
            TopRadius = 0;
            Height = 2;
            ThetaDivisions = 35;

            FillColor = XColor.FromHexString("#FF000080");
        }

        bool showBaseCap;
        [Display(Order = 2)]
        [MarksDirty]
        public bool ShowBaseCap
        {
            get { return showBaseCap; }
            set
            {
                showBaseCap = value;
                OnPropertyChanged(nameof(ShowBaseCap));
            }
        }

        double baseRadius;
        [Display(Order = 3)]
        [MarksDirty]
        public double BaseRadius
        {
            get { return baseRadius; }
            set
            {
                baseRadius = value;
                OnPropertyChanged(nameof(BaseRadius));
            }
        }

        bool showTopCap;
        [Display(Order = 4)]
        [MarksDirty]
        public bool ShowTopCap
        {
            get { return showTopCap; }
            set
            {
                showTopCap = value;
                OnPropertyChanged(nameof(ShowTopCap));
            }
        }

        double topRadius;
        [Display(Order = 5)]
        [MarksDirty]
        public double TopRadius
        {
            get { return topRadius; }
            set
            {
                topRadius = value;
                OnPropertyChanged(nameof(TopRadius));
            }
        }

        double height;
        [Display(Order = 6)]
        [MarksDirty]
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        int thetaDivisions;

        [DisplayName("Divisions")]
        [Display(Order = 7)]
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


        double directionX;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 8)]
        [MarksDirty]
        public double DirectionX
        {
            get { return directionX; }
            set
            {
                directionX = value;
                OnPropertyChanged(nameof(DirectionX));
                OnPropertyChanged(nameof(Direction));
            }
        }

        double directionY;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 9)]
        [MarksDirty]
        public double DirectionY
        {
            get { return directionY; }
            set
            {
                directionY = value;
                OnPropertyChanged(nameof(DirectionY));
                OnPropertyChanged(nameof(Direction));
            }
        }

        double directionZ;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 10)]
        [MarksDirty]
        public double DirectionZ
        {
            get { return directionZ; }
            set
            {
                directionZ = value;
                OnPropertyChanged(nameof(DirectionZ));
                OnPropertyChanged(nameof(Direction));
            }
        }

        [Browsable(false)]
        public XVector3D Direction
        {
            get { return new XVector3D(DirectionX, DirectionY, -DirectionZ); }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 11)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Origin));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 12)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Origin));
            }
        }

        double z;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 13)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double Z
        {
            get { return z; }
            set
            {
                z = value;
                OnPropertyChanged(nameof(Z));
                OnPropertyChanged(nameof(Origin));
            }
        }

        double rotationX;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 14)]
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
        [Display(Order = 15)]
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
        [Display(Order = 16)]
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

        [Browsable(false)]
        public XPoint3D Origin
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

        //public override void PlacingMouseMove(PlacementStatus status, Point _mousePosition)
        //{
        //    var mousePosition = SnapToGrid(_mousePosition);
        //    switch (status)
        //    {
        //        case PlacementStatus.Ready:
        //            X = mousePosition.X;
        //            Y = mousePosition.Y;
        //            break;
        //    }
        //}

        //public override void PlacingMouseUp(PlacementData status, Point _mousePosition)
        //{
        //    var mousePosition = SnapToGrid(_mousePosition);
        //    switch (status.PlacementStatus)
        //    {
        //        //1st click
        //        case PlacementStatus.Ready:
        //            X = mousePosition.X;
        //            Y = mousePosition.Y;

        //            //place
        //            IsPlaced = true;
        //            Parent.OnDrawingChanged();

        //            //create another line
        //            var canvasItem = (ConeMeshItem)Activator.CreateInstance(GetType());
        //            canvasItem.X = mousePosition.X;
        //            canvasItem.Y = mousePosition.Y;
        //            // canvasItem.BorderWidth = BorderWidth;

        //            Parent.PlacingObject = new PlacementData
        //            {
        //                PlacementStatus = PlacementStatus.Ready,
        //                PlacingObjects = new List<ISelectableItem> { canvasItem }
        //            };

        //            Parent.AddItem(canvasItem);
        //            break;

        //    }
        //}

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (Cone3DItem)primitive;

            X = r.OriginX;
            Y = r.OriginY;
            Z = r.OriginZ;
            DirectionX = r.DirectionX;
            DirectionY = r.DirectionY;
            DirectionZ = r.DirectionZ;
            Height = r.Height;
            ThetaDivisions = r.ThetaDivisions;
            ShowBaseCap = r.ShowBaseCap;
            BaseRadius = r.BaseRadius;
            ShowTopCap = r.ShowTopCap;
            TopRadius = r.TopRadius;
            RotationX = r.RotationX;
            RotationY = r.RotationY;
            RotationZ = r.RotationZ;
            FillColor = XColor.FromHexString(r.FillColor);
            PadNumber = r.PadNumber;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new Cone3DItem();

            r.OriginX = X;
            r.OriginY = Y;
            r.OriginZ = Z;
            r.DirectionX = DirectionX;
            r.DirectionY = DirectionY;
            r.DirectionZ = DirectionZ;
            r.Height = Height;
            r.ThetaDivisions = ThetaDivisions;
            r.ShowBaseCap = ShowBaseCap;
            r.BaseRadius = BaseRadius;
            r.ShowTopCap = ShowTopCap;
            r.TopRadius = TopRadius;
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

        public override void Rotate()
        {
            //var r = (rotationZ + 90) % 360;
            RotationZ = RotateSafe(rotationZ);
        }

        public override string ToString()
        {
            return $"Cone ({PadNumber})";
        }
    }
}
