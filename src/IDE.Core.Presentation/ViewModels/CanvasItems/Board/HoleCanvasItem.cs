using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public class HoleCanvasItem : BoardCanvasItemViewModel//BaseCanvasItem
                                , IPlainDesignerItem, IHoleCanvasItem
    {

        public HoleCanvasItem()
        {
            Drill = 0.6;
        }

        private DrillType drillType;

        [MarksDirty]
        [Display(Order = 1)]
        public DrillType DrillType
        {
            get { return drillType; }
            set
            {
                drillType = value;
                OnPropertyChanged(nameof(DrillType));
            }
        }

        double drill;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double Drill
        {
            get { return drill; }
            set
            {
                drill = value;
                OnPropertyChanged(nameof(Drill));
                OnPropertyChanged(nameof(Radius));
                OnPropertyChanged(nameof(Rect));
            }
        }

        #region Slot definition

        double height;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(Rect));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 3)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Center));
                OnPropertyChanged(nameof(Rect));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 4)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Center));
                OnPropertyChanged(nameof(Rect));
            }
        }

        double rot;

        [Display(Order = 5)]
        [MarksDirty]
        public double Rot
        {
            get { return rot; }
            set
            {
                rot = (int)value % 360;
                OnPropertyChanged(nameof(Rot));
            }
        }

        #endregion

        [Browsable(false)]
        public double Radius
        {
            get
            {
                return Drill / 2;
            }
        }


        

        [Browsable(false)]
        public XPoint Center
        {
            get
            {
                return new XPoint(X, Y);
            }
        }

        bool isPlated;

        [MarksDirty]
        [Display(Order = 6)]
        public bool IsPlated
        {
            get { return isPlated; }
            set
            {
                isPlated = value;
                OnPropertyChanged(nameof(IsPlated));
            }
        }

        

        [Browsable(false)]
        public XRect Rect
        {
            get
            {
                return new XRect(-0.5 * drill, -0.5 * height, drill, Height);
            }
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            if (DrillType == DrillType.Slot)
                return new XRect(X - Drill * 0.5, Y - height * 0.5, Drill, height);

            return new XRect(X - Drill * 0.5, Y - Drill * 0.5, Drill, Drill);
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (Hole)primitive;

            Drill = t.drill;
            X = t.x;
            Y = t.y;
            IsPlated = t.IsPlated;

            Height = t.Height;
            Rot = t.Rot;
            DrillType = t.DrillType;
            IsLocked = t.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new Hole();

            t.drill = Drill;
            t.x = X;
            t.y = Y;
            t.IsPlated = IsPlated;

            t.Height = Height;
            t.Rot = Rot;
            t.DrillType = DrillType;
            t.IsLocked = IsLocked;

            return t;
        }

      
        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void TransformBy(XMatrix matrix)
        {
            var c = matrix.Transform(Center);

            X = c.X;
            Y = c.Y;
        }

        protected override XTransform GetLocalRotationTransform()
        {
            return new XRotateTransform(Rot);
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }



        public override void Rotate(double angle = 90)
        {
        }

        public override void LoadLayers()
        {
        }

        public override string ToString()
        {
            return $"Hole ({Drill})";
        }
    }
}
