using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;

namespace IDE.Core.Designers
{
    public class JunctionCanvasItem : NetSegmentCanvasItem
                                    , IJunctionCanvasItem
    {
        public JunctionCanvasItem()
        {
            Diameter = 1;

            Color = XColor.FromHexString("#FF000080");
        }

        double diameter;

        /// <summary>
        /// Size should be fixed
        /// </summary>
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 1)]
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
                return Diameter / 2;
            }
        }

        double x;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 2)]
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
        [Display(Order = 3)]
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

        XColor color;

        [Display(Order = 4)]
        [Editor(EditorNames.XColorEditor, EditorNames.XColorEditor)]
        [MarksDirty]
        public XColor Color
        {
            get { return color; }

            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
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
            return new XRect(X - Diameter * 0.5, Y - Diameter * 0.5, Diameter, Diameter);
        }


        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var t = (Junction)primitive;

            X = t.x;
            Y = t.y;
            Color = XColor.FromHexString(t.Color);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var t = new Junction();

            t.x = X;
            t.y = Y;
            t.Color = Color.ToHexString();

            return t;
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

        public override void Rotate()
        {
        }

        public override string ToString()
        {
            return $"Junction (C:{Center})";
        }
    }
}
