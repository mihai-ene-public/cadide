using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public class CircleBoardCanvasItem : SingleLayerBoardCanvasItem, ICircleCanvasItem, IPlainDesignerItem
    {
        public CircleBoardCanvasItem()
        {
            Diameter = 4;
            BorderWidth = 1;
        }

        double borderWidth;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]
        [MarksDirty]
        public double BorderWidth
        {
            get { return borderWidth; }
            set
            {
                borderWidth = value;
                OnPropertyChanged(nameof(BorderWidth));
            }
        }

        bool isFilled;

        [Display(Order = 3)]
        [MarksDirty]
        public bool IsFilled
        {
            get { return isFilled; }

            set
            {
                isFilled = value;
                OnPropertyChanged(nameof(IsFilled));
            }
        }

        double diameter;
        [Description("Diameter of the circle (supports expressions)")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 4)]
        [MarksDirty]
        public double Diameter
        {
            get { return diameter; }
            set
            {
                diameter = value;
                OnPropertyChanged(nameof(Diameter));
                OnPropertyChanged(nameof(Radius));
            }
        }

        [Browsable(false)]
        public double Radius
        {
            get
            {
                return Diameter * 0.5d;
            }
        }


        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 5)]
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
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 6)]
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

        [Browsable(false)]
        public XPoint Center
        {
            get
            {
                return new XPoint(X, Y);
            }
        }

        public override void Translate(double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(x - diameter * 0.5, y - diameter * 0.5, diameter, diameter);
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var circle = (CircleBoard)primitive;

            Diameter = circle.Diameter;
            X = circle.x;
            Y = circle.y;
            BorderWidth = circle.BorderWidth;
            IsFilled = circle.IsFilled;
            LayerId = circle.layerId;
            IsLocked = circle.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var circle = new CircleBoard();

            circle.Diameter = Diameter;
            circle.x = X;
            circle.y = Y;
            circle.BorderWidth = BorderWidth;
            circle.IsFilled = IsFilled;
            circle.layerId = (Layer?.LayerId).GetValueOrDefault();
            circle.IsLocked = IsLocked;

            return circle;
        }


        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }

        public override void TransformBy(XMatrix matrix)
        {
            var c = matrix.Transform(Center);

            X = c.X;
            Y = c.Y;
        }
        //protected override Transform GetLocalRotationTransform()
        //{
        //    return new RotateTransform(Rot);
        //}

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }

        public override void Rotate(double angle = 90)
        {
        }

        public override string ToString()
        {
            return $"Circle (C:{Center}; D:{Diameter:0.00})";
        }
    }
}
