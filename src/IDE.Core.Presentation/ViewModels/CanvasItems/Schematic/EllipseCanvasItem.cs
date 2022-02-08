using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;

namespace IDE.Core.Designers
{
    public class EllipseCanvasItem : BaseCanvasItem, IPlainDesignerItem, IEllipseSchematicCanvasItem
    {
        public EllipseCanvasItem()
        {
            Width = 20;
            Height = 10;
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


        double width;
        [Description("Diameter of the circle (supports expressions)")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 4)]
        [MarksDirty]
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                //Left = x - width / 2;

                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(RadiusX));
            }
        }

        double height;
        [Description("Diameter of the circle (supports expressions)")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 5)]
        [MarksDirty]
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                //Top = y - height / 2;
                OnPropertyChanged(nameof(Height));
                OnPropertyChanged(nameof(RadiusY));
            }
        }

        [Browsable(false)]
        public double RadiusX
        {
            get
            {
                return Width / 2;
            }
        }

        [Browsable(false)]
        public double RadiusY
        {
            get
            {
                return Height / 2;
            }
        }



        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 6)]
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
        [Display(Order = 7)]
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

        double rot;

        [Display(Order = 11)]
        [MarksDirty]
        public double Rot
        {
            get { return rot; }
            set
            {
                rot = value;

                OnPropertyChanged(nameof(Rot));
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

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            Rot = RotateSafe(Rot, rotAngle);
        }

        public override XRect GetBoundingRectangle()
        {
            return new XRect(X - width * 0.5, Y - height * 0.5, Width, Height);
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var ellipse = (Ellipse)primitive;

            Width = ellipse.Width;
            Height = ellipse.Height;
            X = ellipse.x;
            Y = ellipse.y;
            BorderWidth = ellipse.BorderWidth;
            BorderColor = XColor.FromHexString(ellipse.BorderColor);
            FillColor = XColor.FromHexString(ellipse.FillColor);
            ZIndex = ellipse.ZIndex;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var ellipse = new Ellipse();

            ellipse.Width = Width;
            ellipse.Height = Height;
            ellipse.x = X;
            ellipse.y = Y;
            ellipse.BorderWidth = BorderWidth;
            ellipse.BorderColor = BorderColor.ToHexString();
            ellipse.FillColor = FillColor.ToHexString();
            ellipse.ZIndex = ZIndex;

            return ellipse;
        }

        protected override XTransform GetLocalRotationTransform()
        {
            return new XRotateTransform(Rot);
        }

        protected override XTransform GetLocalTranslationTransform()
        {
            return new XTranslateTransform(X, Y);
        }
        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void Rotate()
        {
            Rot = RotateSafe(Rot);
        }

        public override string ToString()
        {
            return $"Ellipse (C:{Center}; W:{Width}, H:{Height})";
        }
    }
}
