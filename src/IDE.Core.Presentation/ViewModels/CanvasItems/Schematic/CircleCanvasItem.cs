using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using System;

namespace IDE.Core.Designers
{
    public class CircleCanvasItem : BaseCanvasItem, IPlainDesignerItem, ICircleSchematicCanvasItem
    {
        public CircleCanvasItem()
        {
            Diameter = 2;
            BorderWidth = 0.5;
            BorderColor = XColor.FromHexString("#FF000080");
            FillColor = XColors.Transparent;
        }

        double borderWidth;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 1)]
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

        XColor borderColor;

        [Display(Order = 2)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor BorderColor
        {
            get { return borderColor; }

            set
            {
                borderColor = value;
                OnPropertyChanged(nameof(BorderColor));
            }
        }

        XColor fillColor;

        [Display(Order = 3)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor FillColor
        {
            get { return fillColor; }

            set
            {
                fillColor = value;
                OnPropertyChanged(nameof(FillColor));
            }
        }

        [Browsable(false)]
        public bool IsFilled
        {
            get { return fillColor != XColors.Transparent; }
            set
            {
                //    isFilled = value;
                //    OnPropertyChanged(nameof(IsFilled));
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
                return Diameter * 0.5;
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
                x = value.Round(4);
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
                y = value.Round(4);
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

        public override void TransformBy(XMatrix matrix)
        {
            var c = matrix.Transform(Center);

            X = c.X;
            Y = c.Y;
        }

        public override XRect GetBoundingRectangle()
        {
            //todo: borderwidth is ignored ??
            return new XRect(X - Diameter * 0.5, Y - Diameter * 0.5, Diameter, Diameter);
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var circle = (Circle)primitive;

            Diameter = circle.Diameter;
            X = circle.x;
            Y = circle.y;
            BorderWidth = circle.BorderWidth;
            BorderColor = XColor.FromHexString(circle.BorderColor);
            FillColor = XColor.FromHexString(circle.FillColor);
            ZIndex = circle.ZIndex;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var circle = new Circle();

            circle.Diameter = Diameter;
            circle.x = X;
            circle.y = Y;
            circle.BorderWidth = BorderWidth;
            circle.BorderColor = BorderColor.ToHexString();
            circle.FillColor = FillColor.ToHexString();
            circle.ZIndex = ZIndex;

            return circle;
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
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
