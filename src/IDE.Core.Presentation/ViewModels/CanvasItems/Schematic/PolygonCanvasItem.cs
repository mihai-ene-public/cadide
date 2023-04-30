using IDE.Core.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.ComponentModel.DataAnnotations;
using IDE.Core.Types.Attributes;

namespace IDE.Core.Designers
{
    public class PolygonCanvasItem : BaseCanvasItem, IPlainDesignerItem, IPolySchematicCanvasItem
    {

        public PolygonCanvasItem()
        {
            PolygonPoints = new ObservableCollection<XPoint>();

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
        public bool IsFilled { get { return fillColor != XColors.Transparent; } }

        PolygonType polygonType;

        [Display(Order = 6)]
        [Browsable(false)]
        public PolygonType PolygonType
        {
            get { return polygonType; }
            set
            {
                polygonType = value;
                OnPropertyChanged(nameof(PolygonType));
            }
        }


        IList<XPoint> polygonPoints;

        [Browsable(false)]
        public IList<XPoint> PolygonPoints
        {
            get
            {
                return polygonPoints;
            }
            set
            {
                polygonPoints = value;

                OnPropertyChanged(nameof(PolygonPoints));
            }
        }



        public override void Translate(double dx, double dy)
        {
            for (int i = 0; i < PolygonPoints.Count; i++)
            {
                var p = polygonPoints[i];
                p.Offset(dx, dy);
                polygonPoints[i] = p;
            }
            OnPropertyChanged(nameof(PolygonPoints));
        }

        public override void TransformBy(XMatrix matrix)
        {
            for (int i = 0; i < PolygonPoints.Count; i++)
            {
                var p = polygonPoints[i];
                p = matrix.Transform(p);
                polygonPoints[i] = p.Round();
            }
            OnPropertyChanged(nameof(PolygonPoints));
        }

        public override XRect GetBoundingRectangle()
        {
            var minPoint = PolygonPoints.FirstOrDefault();
            var maxPoint = PolygonPoints.FirstOrDefault();

            foreach (var point in PolygonPoints)
            {
                if (point.X < minPoint.X)
                    minPoint.X = point.X;
                if (point.Y < minPoint.Y)
                    minPoint.Y = point.Y;
                if (point.X > maxPoint.X)
                    maxPoint.X = point.X;
                if (point.Y > maxPoint.Y)
                    maxPoint.Y = point.Y;
            }

            return new XRect(minPoint, maxPoint);
        }


        protected override void LoadFromPrimitiveInternal(IPrimitive primitive)
        {
            var p = (Polygon)primitive;

            var vertices = p.vertices.Select(v => new XPoint(v.x, v.y)).ToList();
            PolygonPoints = new ObservableCollection<XPoint>(vertices);
            BorderWidth = p.BorderWidth;
            BorderColor = XColor.FromHexString(p.BorderColor);
            FillColor = XColor.FromHexString(p.FillColor);
        }

        public override IPrimitive SaveToPrimitive()
        {
            var p = new Polygon();

            p.vertices = polygonPoints.Select(v => new Vertex { x = v.X, y = v.Y }).ToList();
            p.BorderWidth = BorderWidth;
            p.BorderColor = BorderColor.ToHexString();
            p.FillColor = FillColor.ToHexString();

            return p;
        }

        public override void MirrorX()
        {
            ScaleX *= -1;
        }

        public override void MirrorY()
        {
            ScaleY *= -1;
        }

        public override void Rotate(double angle = 90)
        {
            var mp = new XPoint();

            //start point is equal endpoint so the actual number of points is n-1
            var points = polygonPoints.Take(polygonPoints.Count - 1).ToList();
            foreach (var p in points)
            {
                mp.Offset(p.X, p.Y);
            }
            mp.X /= points.Count;
            mp.Y /= points.Count;

            var tg = new XTransformGroup();
            var rotateTransform = new XRotateTransform(angle)
            {
                CenterX = mp.X,
                CenterY = mp.Y
            };

            tg.Children.Add(rotateTransform);

            TransformBy(tg.Value);
        }

        public override string ToString()
        {
            return $"Poly";
        }
    }
}
