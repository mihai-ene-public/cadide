using IDE.Core.Storage;
using System.ComponentModel;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;
using System;

namespace IDE.Core.Designers
{


    public class LineBoardCanvasItem : SingleLayerBoardCanvasItem, ILineCanvasItem, IPlainDesignerItem
    {
        public LineBoardCanvasItem() : base()
        {
            //should default to current layer

            Width = 0.2;
        }

        double width;

        /// <summary>
        /// Width of the wire in mm
        /// </summary>
        [Description("Width (thickness) of the line (supports expressions)")]
        [Editor(EditorNames.SizeMilimetersUnitsEditor, EditorNames.SizeMilimetersUnitsEditor)]
        [Display(Order = 2)]//1 is the Layer
        [MarksDirty]
        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        double x1;

        [Display(Order = 2)]
        [Description("X coordonate of the first point defining the line (supports expressions)")]
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [MarksDirty]
        public double X1
        {
            get { return x1; }
            set
            {
                //  x1 = SnapToGrid(value);
                x1 = value.Round(4);
                OnPropertyChanged(nameof(X1));
            }
        }

        double y1;

        [Description("Y coordonate of the first point defining the line (supports expressions)")]
        [Display(Order = 3)]
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [MarksDirty]
        public double Y1
        {
            get { return y1; }
            set
            {
                // y1 = SnapToGrid(value);
                y1 = value.Round(4);

                OnPropertyChanged(nameof(Y1));
            }
        }

        double x2;

        [Description("X coordonate of the second point defining the line (supports expressions)")]
        [Display(Order = 4)]
        [Editor(EditorNames.PositionXUnitsEditor, EditorNames.PositionXUnitsEditor)]
        [MarksDirty]
        public double X2
        {
            get { return x2; }
            set
            {
                //x2 = SnapToGrid(value);
                x2 = value.Round(4);

                OnPropertyChanged(nameof(X2));
            }
        }

        double y2;

        [Description("Y coordonate of the second point defining the line (supports expressions)")]
        [Display(Order = 5)]
        [Editor(EditorNames.PositionYUnitsEditor, EditorNames.PositionYUnitsEditor)]
        [MarksDirty]
        public double Y2
        {
            get { return y2; }
            set
            {
                y2 = value.Round(4);
                OnPropertyChanged(nameof(Y2));
            }
        }




        public override void Translate(double dx, double dy)
        {
            X1 += dx;
            Y1 += dy;

            X2 += dx;
            Y2 += dy;
        }

        public override XRect GetBoundingRectangle()
        {
            var r = new XRect(new XPoint(x1, y1), new XPoint(x2, y2));
            r.Inflate(0.5 * width);
            return r;
        }

        public override IPrimitive SaveToPrimitive()
        {
            var wire = new LineBoard();

            wire.x1 = X1;
            wire.x2 = X2;
            wire.y1 = Y1;
            wire.y2 = Y2;
            wire.width = Width;
            wire.layerId = LayerId;
            wire.IsLocked = IsLocked;

            return wire;
        }

        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var wire = (LineBoard)primitive;

            X1 = wire.x1;
            X2 = wire.x2;
            Y1 = wire.y1;
            Y2 = wire.y2;
            Width = wire.width;
            LayerId = wire.layerId;
            IsLocked = wire.IsLocked;
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
            var sp = new XPoint(x1, y1);
            var ep = new XPoint(x2, y2);

            sp = matrix.Transform(sp);
            ep = matrix.Transform(ep);

            X1 = sp.X;
            Y1 = sp.Y;

            X2 = ep.X;
            Y2 = ep.Y;
        }

        public override void Rotate()
        {
            if (!IsPlaced)
                return;

            //rotate by 90 deg around middle point
            var mp = new XPoint(0.5 * (x1 + x2), 0.5 * (y1 + y2));
            var tg = new XTransformGroup();
            var rotateTransform = new XRotateTransform(90)
            {
                CenterX = mp.X,
                CenterY = mp.Y
            };

            tg.Children.Add(rotateTransform);

            TransformBy(tg.Value);
        }

        public override string ToString()
        {
            return $"Line ({X1},{Y1}; {X2},{Y2})";
        }
    }
}
