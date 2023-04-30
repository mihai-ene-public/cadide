using IDE.Core.Storage;
using System;
using System.ComponentModel;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using IDE.Core.Interfaces;

namespace IDE.Core.Designers
{
    public class TextMeshItem : BaseMeshItem
    {
        public TextMeshItem()
        {
            Text = "Text";
            //TextDecoration = TextDecorationEnum.None;
            FontSize = 24;
            FontFamily = "Segoe UI";
            FillColor = XColors.White;
            //RotationX = 90;
            Height = 1;
        }

        string fontFamily;

        [Display(Order = 2)]
        [Editor(EditorNames.FontFamilyEditor, EditorNames.FontFamilyEditor)]
        [MarksDirty]
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                fontFamily = value;
                OnPropertyChanged(nameof(FontFamily));
            }
        }

        double fontSize;

        [Display(Order = 3)]
        [MarksDirty]
        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        bool bold;

        [Display(Order = 4)]
        [MarksDirty]
        public bool Bold
        {
            get { return bold; }
            set
            {
                bold = value;
                OnPropertyChanged(nameof(Bold));
            }
        }

        double height;

        [Display(Order = 5)]
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

        string text;

        [Editor(EditorNames.MultilineTextEditor, EditorNames.MultilineTextEditor)]
        [Display(Order = 6)]
        [MarksDirty]
        public string Text
        {
            get { return text; }
            set
            {
                text = value;

                OnPropertyChanged(nameof(Text));
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Display(Order = 7)]
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
        [Display(Order = 8)]
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
        [Display(Order = 9)]
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
        [Display(Order = 10)]
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
        [Display(Order = 11)]
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
        [Display(Order = 12)]
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
            var t = (Text3DItem)primitive;

            X = t.CenterX;
            Y = t.CenterY;
            Z = t.CenterZ;
            RotationX = t.RotationX;
            RotationY = t.RotationY;
            RotationZ = t.RotationZ;
            PadNumber = t.PadNumber;
            Text = t.Value;
            //TextAlign = t.TextAlign;
            Bold = t.Bold;
            //Italic = t.Italic;
            FontFamily = t.FontFamily;
            FontSize = t.FontSize;
            Height = t.Height;

            FillColor = XColor.FromHexString(t.FillColor);

        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new Text3DItem();

            t.CenterX = X;
            t.CenterY = Y;
            t.CenterZ = Z;
            t.RotationX = RotationX;
            t.RotationY = RotationY;
            t.RotationZ = RotationZ;
            t.FillColor = FillColor.ToHexString();
            t.PadNumber = PadNumber;

            t.Value = Text;
           // t.TextAlign = TextAlign;
            t.Bold = Bold;
           // t.Italic = Italic;
            t.FontFamily = FontFamily;
            t.FontSize = FontSize;
            t.Height = Height;

            return t;
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
            //trim the text to show for a single line
            var maxLen = 10;
            var t = Text.Replace(Environment.NewLine, " ");
            if (t.Length > maxLen)
                t = Text.Substring(0, maxLen);

            return $"Text ({t})";
        }
    }
}
