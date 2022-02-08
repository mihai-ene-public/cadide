using System.ComponentModel;
using IDE.Core.Storage;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;

namespace IDE.Core.Designers
{
    public class ImageCanvasItem : BaseCanvasItem, IPlainDesignerItem, IImageCanvasItem
    {
        public ImageCanvasItem()
        {
            Width = 5;
            Height = 5;
            Stretch = XStretch.Uniform;
            BorderWidth = 0.5;
            BorderColor = XColor.FromHexString("#FF000080");
            FillColor =XColors.Transparent;
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

        double cornerRadius;

        [Display(Order = 3)]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [MarksDirty]
        public double CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                cornerRadius = value;
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        XColor fillColor;

        [Display(Order = 4)]
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



        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 5)]
        [MarksDirty]
        public double Width
        {
            get { return width; }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        double height;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
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

        byte[] imageBytes;

        [Editor(EditorNames.BytesArrayEditor, EditorNames.BytesArrayEditor)]
        [Display(Order = 7)]
        [MarksDirty]
        public byte[] ImageBytes
        {
            get { return imageBytes; }
            set
            {
                imageBytes = value;
                OnPropertyChanged(nameof(ImageBytes));
            }
        }


        XStretch stretch;

        [Display(Order = 8)]
        [MarksDirty]
        public XStretch Stretch
        {
            get { return stretch; }
            set
            {
                stretch = value;
                OnPropertyChanged(nameof(Stretch));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 9)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 10)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }

        double width;

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
            var p = new XPoint(x, y);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            Rot = RotateSafe(Rot, rotAngle);

        }

        public override XRect GetBoundingRectangle()
        {
            //this should be relative to canvas coordinates
            return new XRect(X, Y, Width, Height);
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (ImagePrimitive)primitive;

            X = r.X;
            Y = r.Y;
            Rot = r.Rot;
            ScaleX = r.ScaleX;
            ScaleY = r.ScaleY;
            Width = r.Width;
            Height = r.Height;
            BorderWidth = r.BorderWidth;
            BorderColor = XColor.FromHexString(r.BorderColor);
            FillColor =   XColor.FromHexString(r.FillColor);
            CornerRadius = r.CornerRadius;
            Stretch = r.StretchMode;
            if (r.Image != null)
                ImageBytes = r.Image.Bytes;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new ImagePrimitive();

            r.X = X;
            r.Y = Y;
            r.Rot = Rot;
            r.ScaleX = ScaleX;
            r.ScaleY = ScaleY;
            r.Width = Width;
            r.Height = Height;
            r.BorderWidth = BorderWidth;
            r.BorderColor = BorderColor.ToHexString();
            r.FillColor = FillColor.ToHexString();
            r.CornerRadius = CornerRadius;
            r.StretchMode = Stretch;
            r.Image = new EmbeddedImage
            {
                Bytes = ImageBytes
            };

            return r;
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
            return "Image";
        }
    }

}
