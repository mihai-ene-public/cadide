using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public class ArcBoardCanvasItem : SingleLayerBoardCanvasItem, IArcCanvasItem
                                        , IPlainDesignerItem
    {
        public ArcBoardCanvasItem()
        {
            BorderWidth = 0.2;
            Radius = 2;
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

        XSweepDirection sweepDirection;

        [Display(Order = 4)]
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

        [Display(Order = 5)]
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

        [Display(Order = 6)]
        [Browsable(false)]
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

        [Browsable(false)]
        public XSize Size
        {
            get { return new XSize(Radius, Radius); }
        }

        double radius;
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 7)]
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

        [Browsable(false)]
        public XPoint StartPoint
        {
            get { return new XPoint(startPointX, startPointY); }
        }

        double startPointX;

        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [Display(Order = 8)]
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
        [Display(Order = 9)]
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
        [Display(Order = 10)]
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
        [Display(Order = 11)]
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

        [Browsable(false)]
        public XPoint EndPoint
        {
            get { return new XPoint(endPointX, endPointY); }
        }

        public override void Translate(double dx, double dy)
        {
            //var newSP = StartPoint;
            //var newEP = EndPoint;

            //newSP.Offset(dx, dy);
            //newEP.Offset(dx, dy);

            //StartPoint = newSP;
            //EndPoint = newEP;

            StartPointX += dx;
            StartPointY += dy;

            EndPointX += dx;
            EndPointY += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            var r = Geometry2DHelper.GetArcBoundingBox(StartPoint, EndPoint, radius, sweepDirection);
            r.Inflate(0.5 * borderWidth);

            return r;
        }

        //public override void PlacingMouseMove(PlacementStatus status, Point mousePosition)
        //{
        //    var mpSnapped = SnapToGrid(mousePosition);
        //    switch (status)
        //    {
        //        case PlacementStatus.Ready:
        //            StartPointX = EndPointX = mpSnapped.X;
        //            StartPointY = EndPointY = mpSnapped.Y;
        //            break;
        //        case PlacementStatus.Started:
        //            EndPointX = mpSnapped.X;
        //            EndPointY = mpSnapped.Y;
        //            break;
        //    }
        //}

        //public override void PlacingMouseUp(PlacementData status, Point mousePosition)
        //{
        //    var mpSnapped = SnapToGrid(mousePosition);

        //    switch (status.PlacementStatus)
        //    {
        //        //1st click
        //        case PlacementStatus.Ready:
        //            StartPointX = mpSnapped.X;
        //            StartPointY = mpSnapped.Y;
        //            status.PlacementStatus = PlacementStatus.Started;
        //            break;
        //        //2nd click
        //        case PlacementStatus.Started:
        //            EndPointX = mpSnapped.X;
        //            EndPointY = mpSnapped.Y;
        //            IsPlaced = true;
        //            Parent.OnDrawingChanged();

        //            //create another line
        //            var canvasItem = (ArcBoardCanvasItem)Activator.CreateInstance(GetType());
        //            canvasItem.StartPointX = mpSnapped.X;
        //            canvasItem.StartPointY = mpSnapped.Y;
        //            canvasItem.EndPointX = mpSnapped.X;
        //            canvasItem.EndPointY = mpSnapped.Y;
        //            canvasItem.Radius = Radius;
        //            canvasItem.BorderWidth = BorderWidth;
        //            canvasItem.IsFilled = IsFilled;
        //            canvasItem.SweepDirection = SweepDirection;
        //            canvasItem.isLargeArc = IsLargeArc;
        //            canvasItem.RotationAngle = RotationAngle;
        //            canvasItem.Layer = Layer;

        //            Parent.PlacingObject = new PlacementData
        //            {
        //                PlacementStatus = PlacementStatus.Started,
        //                PlacingObjects = new List<ISelectableItem> { canvasItem }
        //            };

        //            Parent.AddItem(canvasItem);

        //            break;
        //    }
        //}

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (ArcBoard)primitive;

            StartPointX = p.StartPoint.X;
            StartPointY = p.StartPoint.Y;
            EndPointX = p.EndPoint.X;
            EndPointY = p.EndPoint.Y;
            Radius = p.SizeDiameter;

            BorderWidth = p.BorderWidth;

            IsFilled = p.IsFilled;
            SweepDirection = p.SweepDirection;
            IsLargeArc = p.IsLargeArc;
            RotationAngle = p.RotationAngle;
            LayerId = p.layerId;
            IsLocked = p.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new ArcBoard();

            p.IsLocked = IsLocked;
            p.StartPoint = StartPoint;
            p.EndPoint = EndPoint;
            p.SizeDiameter = Radius;

            p.BorderWidth = BorderWidth;

            p.IsFilled = IsFilled;
            p.SweepDirection = SweepDirection;
            p.IsLargeArc = IsLargeArc;
            p.RotationAngle = RotationAngle;
            p.layerId = (Layer?.LayerId).GetValueOrDefault();

            return p;
        }

        //public override Adorner CreateAdorner(UIElement element)
        //{
        //    return new ArcAdorner(element);
        //}

        public override void MirrorX()
        {
        }

        public override void MirrorY()
        {
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

        public override void Rotate()
        {
            //todo: see if we really need this check
            if (!IsPlaced)
                return;

            //rotate by 90 deg around circle center point

            var mp = GetCenter();
            var tg = new XTransformGroup();
            var rotateTransform = new XRotateTransform(90)
            {
                CenterX = mp.X,
                CenterY = mp.Y
            };

            tg.Children.Add(rotateTransform);

            TransformBy(tg.Value);
        }

        //returns the center of the ellipse
        //we need to fix this: it is working well for a circle but not an ellipse
        public XPoint GetCenter()//todo: duplicated with SchematicArcCanvasItem
        {
            //http://www.charlespetzold.com/blog/2008/01/Mathematics-of-ArcSegment.html

            var radiusX = Size.Width;
            var radiusY = Size.Height;
            var matx = new XMatrix();
            matx.Scale(radiusY / radiusX, 1);
            var sp = matx.Transform(StartPoint);
            var ep = matx.Transform(EndPoint);

            // Get info about chord that connects both points
            var midPoint = new XPoint((sp.X + ep.X) / 2, (sp.Y + ep.Y) / 2);
            var vect = ep - sp;
            double halfChord = vect.Length / 2;

            // Get vector from chord to center
            XVector vectRotated;

            var isClockwise = SweepDirection == XSweepDirection.Clockwise;
            // (comparing two Booleans here!)
            //if (isLargeArc == isCounterclockwise)
            if (isClockwise)
                vectRotated = new XVector(-vect.Y, vect.X);
            else
                vectRotated = new XVector(vect.Y, -vect.X);

            vectRotated.Normalize();

            //the larger between radiuses
            var r = Math.Max(radiusX, radiusY);

            // Distance from chord to center 
            double centerDistance = Math.Sqrt(r * r - halfChord * halfChord);

            if (centerDistance.IsNaN())
                centerDistance = 0.0;

            // Calculate center point
            var center = midPoint + centerDistance * vectRotated;
            //center.X -= radiusX;
            //center.Y -= radiusY;
            return center;
            // return new Point(9, 10);

            ////distance between points
            //var d = (sp - ep).Length;
            ////distance of center from midpoint
            //var h = Math.Sqrt(Math.Pow(radiusY, 2.0d) - d * d / 4.0d);
            //var dir = SweepDirection == SweepDirection.Counterclockwise ? 1 : -1;
            //var u = (ep.X - sp.X) / d;
            //var v = (ep.Y - sp.Y) / d;
            //var cx = 0.5 * (sp.X + ep.X) - dir * h * v;
            //var cy = 0.5 * (sp.Y + ep.Y) + dir * h * u;
            //cx -= sp.X;
            //cy -= sp.Y;

            //return new Point(cx, cy);
        }

        public override string ToString()
        {
            return $"Arc ({StartPoint}; {EndPoint})";
        }
    }
}
