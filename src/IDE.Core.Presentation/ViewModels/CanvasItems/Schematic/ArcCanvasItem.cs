using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using System;

namespace IDE.Core.Designers
{
    public class ArcCanvasItem : BaseCanvasItem, IPlainDesignerItem, IArcSchematicCanvasItem
    {

        public ArcCanvasItem()
        {
            BorderWidth = 0.5;
            BorderColor = XColor.FromHexString("#FF000080");
            FillColor = XColors.Transparent;
            Radius = 2;
        }



        double borderWidth;

        [Description(" (supports expressions)")]
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

        bool isFilled;
        [Display(Order = 4)]
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

        XSweepDirection sweepDirection;
        [Display(Order = 5)]
        [MarksDirty]
        public XSweepDirection SweepDirection
        {
            get { return sweepDirection; }
            set
            {
                sweepDirection = value;
                OnPropertyChanged(nameof(SweepDirection));
            }
        }

        bool isLargeArc;
        [Display(Order = 6)]
        [MarksDirty]
        public bool IsLargeArc
        {
            get { return isLargeArc; }
            set
            {
                isLargeArc = value;
                OnPropertyChanged(nameof(IsLargeArc));
            }
        }

        double rotationAngle;
        [Display(Order = 7)]
        [MarksDirty]
        public double RotationAngle
        {
            get { return rotationAngle; }
            set
            {
                rotationAngle = value;
                OnPropertyChanged(nameof(RotationAngle));
            }
        }

        //Size size;
        [Browsable(false)]
        public XSize Size
        {
            get { return new XSize(Radius, Radius); }
        }

        double radius;

        [Description(" (supports expressions)")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 8)]
        [MarksDirty]
        public double Radius
        {
            get { return radius; }
            set
            {
                radius = value;

                OnPropertyChanged(nameof(Radius));
                OnPropertyChanged(nameof(Size));
            }
        }

        LineCap lineCap;

        [Display(Order = 8)]
        [MarksDirty]
        public LineCap LineCap
        {
            get { return lineCap; }
            set
            {
                lineCap = value;
                OnPropertyChanged(nameof(LineCap));
            }
        }

        [Browsable(false)]
        public XPoint StartPoint
        {
            get { return new XPoint(startPointX, startPointY); }
        }

        [Browsable(false)]
        public XPoint EndPoint
        {
            get { return new XPoint(endPointX, endPointY); }
        }

        double startPointX;

        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 10)]
        [MarksDirty]
        public double StartPointX
        {
            get { return startPointX; }
            set
            {
                startPointX = value;
                OnPropertyChanged(nameof(StartPointX));
                OnPropertyChanged(nameof(StartPoint));
            }
        }

        double startPointY;
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 11)]
        [MarksDirty]
        public double StartPointY
        {
            get { return startPointY; }
            set
            {
                startPointY = value;
                OnPropertyChanged(nameof(StartPointY));
                OnPropertyChanged(nameof(StartPoint));
            }
        }

        double endPointX;
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 12)]
        [MarksDirty]
        public double EndPointX
        {
            get { return endPointX; }
            set
            {
                endPointX = value;
                OnPropertyChanged(nameof(EndPointX));
                OnPropertyChanged(nameof(EndPoint));
            }
        }

        double endPointY;
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 13)]
        [MarksDirty]
        public double EndPointY
        {
            get { return endPointY; }
            set
            {
                endPointY = value;
                OnPropertyChanged(nameof(EndPointY));
                OnPropertyChanged(nameof(EndPoint));
            }
        }

        public override void Translate(double dx, double dy)
        {
            StartPointX += dx;
            StartPointY += dy;

            EndPointX += dx;
            EndPointY += dy;
        }

        public override void TransformBy(XMatrix matrix)
        {
            var sp = matrix.Transform(StartPoint);
            var ep = matrix.Transform(EndPoint);

            StartPointX = sp.X.Round(4);
            StartPointY = sp.Y.Round(4);

            EndPointX = ep.X.Round(4);
            EndPointY = ep.Y.Round(4);
        }

        public override XRect GetBoundingRectangle()
        {
            var r = Geometry2DHelper.GetArcBoundingBox(StartPoint, EndPoint, radius, sweepDirection);
            r.Inflate(0.5 * borderWidth);

            return r;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (Arc)primitive;

            StartPointX = p.StartPoint.X;
            StartPointY = p.StartPoint.Y;
            EndPointX = p.EndPoint.X;
            EndPointY = p.EndPoint.Y;
            Radius = p.Size.Width;

            BorderWidth = p.BorderWidth;
            BorderColor = XColor.FromHexString(p.BorderColor);
            FillColor = XColor.FromHexString(p.FillColor);

            IsFilled = p.IsFilled;
            SweepDirection = p.SweepDirection;
            IsLargeArc = p.IsLargeArc;
            RotationAngle = p.RotationAngle;
            LineCap = p.LineCap;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new Arc();

            p.StartPoint = new XPoint(StartPointX, StartPointY);
            p.EndPoint = new XPoint(EndPointX, EndPointY);
            p.Size = Size;

            p.BorderWidth = BorderWidth;
            p.BorderColor = BorderColor.ToHexString();
            p.FillColor = FillColor.ToHexString();

            p.IsFilled = IsFilled;
            p.SweepDirection = SweepDirection;
            p.IsLargeArc = IsLargeArc;
            p.RotationAngle = RotationAngle;
            p.LineCap = LineCap;

            return p;
        }


        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
        }


        public override void Rotate(double angle = 90)
        {
            //rotate by 90 deg around circle center point

            var mp = GetCenter();
            var tg = new XTransformGroup();
            var rotateTransform = new XRotateTransform(angle)
            {
                CenterX = mp.X,
                CenterY = mp.Y
            };

            tg.Children.Add(rotateTransform);

            TransformBy(tg.Value);

        }

        //returns the center of the ellipse
        //we need to fix this: it is working well for a circle but not an ellipse
        public XPoint GetCenter()
        {
            var center = Geometry2DHelper.GetArcCenter(StartPoint, EndPoint, Size.Width, Size.Height, sweepDirection);
            return center;
        }

        public override string ToString()
        {
            return $"Arc ({StartPoint}; {EndPoint})";
        }
    }
}
