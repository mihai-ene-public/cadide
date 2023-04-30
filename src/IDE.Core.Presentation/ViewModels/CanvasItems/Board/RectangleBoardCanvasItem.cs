using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Attributes;
using IDE.Core.Types.Media;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IDE.Core.Designers
{
    public class RectangleBoardCanvasItem : SingleLayerBoardCanvasItem, IRectangleCanvasItem, IPlainDesignerItem
    {
        public RectangleBoardCanvasItem()
        {
            Width = 5;
            Height = 5;
            BorderWidth = 0.254;
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


        double cornerRadius;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 4)]
        [MarksDirty]
        public double CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                if (cornerRadius == value)
                    return;
                cornerRadius = value;
                OnPropertyChanged(nameof(CornerRadius));
            }
        }

        double width;

        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 5)]
        [MarksDirty]
        public double Width
        {
            get { return width; }
            set
            {
                if (width == value) return;
                width = value;
                OnPropertyChanged(nameof(Width));
                OnPropertyChanged(nameof(Rect));
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
                if (height == value)
                    return;
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
        [Display(Order = 7)]
        [MarksDirty]
        public double X
        {
            get { return x; }
            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
                OnPropertyChanged(nameof(Rect));
            }
        }

        double y;

        /// <summary>
        /// Location X of rectangle in current coordinate system
        /// </summary>
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [Display(Order = 8)]
        [MarksDirty]
        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
                OnPropertyChanged(nameof(Rect));
            }
        }



        [Browsable(false)]
        public XRect Rect
        {
            get
            {
                return new XRect(-0.5 * width, -0.5 * height, Width, Height);
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

        public override XRect GetBoundingRectangle()
        {
            //this should be relative to canvas coordinates
            return new XRect(X - 0.5 * width, Y - 0.5 * height, Width, Height);
        }

        //const double minWidth = 2;
        //const double minHeight = 2;
        //public override void PlacingMouseMove(PlacementStatus status, Point _mousePosition)
        //{
        //    var mousePosition = SnapToGrid(_mousePosition);
        //    switch (status)
        //    {
        //        case PlacementStatus.Ready:
        //            X = mousePosition.X;
        //            Y = mousePosition.Y;
        //            break;
        //        case PlacementStatus.Started:
        //            var w = mousePosition.X - X;
        //            var h = mousePosition.Y - Y;
        //            if (w < minWidth)
        //                w = minWidth;
        //            if (h < minHeight)
        //                h = minHeight;
        //            Width = w;
        //            Height = h;
        //            break;
        //    }
        //}

        //public override void PlacingMouseUp(PlacementData status, Point _mousePosition)
        //{
        //    var mousePosition = SnapToGrid(_mousePosition);
        //    switch (status.PlacementStatus)
        //    {
        //        //1st click
        //        case PlacementStatus.Ready:
        //            X = mousePosition.X;
        //            Y = mousePosition.Y;
        //            status.PlacementStatus = PlacementStatus.Started;
        //            break;
        //        //2nd click
        //        case PlacementStatus.Started:
        //            var w = mousePosition.X - X;
        //            var h = mousePosition.Y - Y;
        //            if (w > minWidth && h > minHeight)
        //            {
        //                Width = w;
        //                Height = h;
        //                IsPlaced = true;
        //                Parent.OnDrawingChanged();

        //                //create another line
        //                var canvasItem = (RectangleBoardCanvasItem)Activator.CreateInstance(GetType());
        //                canvasItem.X = mousePosition.X;
        //                canvasItem.Y = mousePosition.Y;
        //                canvasItem.BorderWidth = BorderWidth;
        //                canvasItem.Layer = Layer;
        //                //canvasItem.Layer = LayersWindow.SelectedLayer;

        //                Parent.PlacingObject = new PlacementData
        //                {
        //                    PlacementStatus = PlacementStatus.Ready,
        //                    PlacingObjects = new List<ISelectableItem> { canvasItem }
        //                };

        //                Parent.AddItem(canvasItem);
        //            }
        //            break;
        //    }
        //}

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var r = (RectangleBoard)primitive;

            X = r.X;
            Y = r.Y;
            Width = r.Width;
            Height = r.Height;
            Rot = r.rot;
            BorderWidth = r.BorderWidth;
            IsFilled = r.IsFilled;
            CornerRadius = r.CornerRadius;
            LayerId = r.layerId;
            IsLocked = r.IsLocked;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var r = new RectangleBoard();

            r.X = X;
            r.Y = Y;
            r.Width = Width;
            r.Height = Height;
            r.rot = Rot;
            r.BorderWidth = BorderWidth;
            r.IsFilled = IsFilled;
            r.CornerRadius = CornerRadius;
            //r.RadiusY = RadiusY;
            r.layerId = (Layer?.LayerId).GetValueOrDefault();
            r.IsLocked = IsLocked;

            return r;
        }

        public static double[] GetDefaultStrokeThicknessValues()
        {
            return new double[] { 10, 12, 16, 24 };
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
            var p = new XPoint(x, y);
            p = matrix.Transform(p);
            X = p.X;
            Y = p.Y;

            var rotAngle = GetRotationAngleFromMatrix(matrix);
            Rot = RotateSafe(Rot, rotAngle);
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
            Rot = RotateSafe(Rot, angle);
        }

        public override string ToString()
        {
            return $"Rectangle ({Rect})";
        }
    }
}
