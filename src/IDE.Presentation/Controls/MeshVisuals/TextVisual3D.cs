using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace IDE.Controls
{
    public class TextVisual3D : AbstractMeshGeometryModel3D
    {
        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily", typeof(FontFamily), typeof(TextVisual3D), new UIPropertyMetadata(new FontFamily("Arial"), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(12.0, GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight", typeof(FontWeight), typeof(TextVisual3D), new UIPropertyMetadata(FontWeights.Normal, GeometryChanged));

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
           "FontStyle", typeof(FontStyle), typeof(TextVisual3D), new UIPropertyMetadata(FontStyles.Normal, GeometryChanged));

        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
          "Height", typeof(double), typeof(TextVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        ///// <summary>
        ///// Identifies the <see cref="Position"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        //    "Position",
        //    typeof(Point3D),
        //    typeof(TextVisual3D),
        //    new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="TextDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextDirectionProperty = DependencyProperty.Register(
            "TextDirection",
            typeof(Vector3D),
            typeof(TextVisual3D),
            new UIPropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(TextVisual3D), new UIPropertyMetadata("Text", GeometryChanged));

        /// <summary>
        /// Identifies the <see cref="UpDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "UpDirection",
            typeof(Vector3D),
            typeof(TextVisual3D),
            new UIPropertyMetadata(new Vector3D(0, 0, -1), GeometryChanged));

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(FontFamilyProperty);
            }

            set
            {
                this.SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font (if not set, the Height property is used.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get
            {
                return (double)this.GetValue(FontSizeProperty);
            }
            set
            {
                this.SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(FontWeightProperty);
            }

            set
            {
                this.SetValue(FontWeightProperty, value);
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                return (FontStyle)GetValue(FontStyleProperty);
            }

            set
            {
                SetValue(FontStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the text.
        /// </summary>
        /// <value>The text height.</value>
        public double Height
        {
            get
            {
                return (double)this.GetValue(HeightProperty);
            }

            set
            {
                this.SetValue(HeightProperty, value);
            }
        }


        ///// <summary>
        ///// Gets or sets the position of the text.
        ///// </summary>
        ///// <value>The position.</value>
        //public Point3D Position
        //{
        //    get
        //    {
        //        return (Point3D)this.GetValue(PositionProperty);
        //    }

        //    set
        //    {
        //        this.SetValue(PositionProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }

            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3D TextDirection
        {
            get
            {
                return (Vector3D)this.GetValue(TextDirectionProperty);
            }

            set
            {
                this.SetValue(TextDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the up direction of the text.
        /// </summary>
        /// <value>The up direction.</value>
        public Vector3D UpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(UpDirectionProperty);
            }

            set
            {
                this.SetValue(UpDirectionProperty, value);
            }
        }

        protected override HelixToolkit.Wpf.SharpDX.Geometry3D Tessellate()
        {
            var textDirection = TextDirection;
            var upDirection = UpDirection;
            var upV3 = new SharpDX.Vector3((float)upDirection.X, (float)upDirection.Y, (float)upDirection.Z);
            var p1 = (float)Height * upV3;

            var g = TextExtruder.ExtrudeText(Text,
                                            FontFamily,
                                            FontSize,
                                            FontStyle,
                                            FontWeight,
                                            new SharpDX.Vector3((float)textDirection.X, (float)textDirection.Y, (float)textDirection.Z),
                                            SharpDX.Vector3.Zero,
                                            p1
                                            );

            return g;
        }
    }

}
