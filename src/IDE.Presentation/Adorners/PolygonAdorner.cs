using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Core.Adorners
{
    public class PolygonAdorner : BaseCanvasItemAdorner
    {
        List<Thumb> vertexThumbs;

        IPolygonCanvasItem polygon;


        public PolygonAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            vertexThumbs = new List<Thumb>();

            polygon = ((FrameworkElement)AdornedElement).DataContext as IPolygonCanvasItem;
            polygon.PropertyChanged += Polygon_PropertyChanged;

            for (int vertexIndex = 0; vertexIndex < polygon.PolygonPoints.Count; vertexIndex++)
            {
                var thumb = new Thumb
                {
                    Cursor = Cursors.SizeAll,
                    Width = 4,
                    Height = 4,
                    Opacity = 0,
                    Background = Brushes.Transparent,
                    Tag = vertexIndex
                };
                thumb.DragDelta += Thumb_DragDelta;
                visualChildren.Add(thumb);
                vertexThumbs.Add(thumb);
            }

            //translate
            //translateThumb = new Thumb
            //{
            //    Cursor = Cursors.SizeAll,
            //    Width = 4,
            //    Height = 4,
            //    Opacity = 0,
            //    Background = Brushes.Transparent,
            //};
            //translateThumb.DragDelta += translateThumb_DragDelta;
            //visualChildren.Add(translateThumb);
        }


        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var vIndex = (int)thumb.Tag;
            polygon.PolygonPoints[vIndex] = position;//new Point(position.X, position.Y);
            polygon.OnPropertyChanged(nameof(polygon.PolygonPoints));
            InvalidateVisual();
        }

        //void translateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        //{
        //    var lastMiddlePoint = GetTranslationRectangle().GetCenter();
        //    //Point position = polygon.SnapToGrid(Mouse.GetPosition(this));
        //    var newLocation = polygon.SnapToGrid(Mouse.GetPosition((IInputElement)VisualTreeHelper.GetParent(AdornedElement)));

        //    var delta = newLocation - lastMiddlePoint;

        //    //offset all points by delta
        //    for (int i = 0; i < polygon.PolygonPoints.Count; i++)
        //    {
        //        var p = polygon.PolygonPoints[i];
        //        p.Offset(delta.X, delta.Y);
        //        polygon.PolygonPoints[i] = p;
        //    }

        //    polygon.OnPropertyChanged(nameof(polygon.PolygonPoints));
        //    InvalidateVisual();
        //}

        void Polygon_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(polygon.IsSelected))
            {
                if (!polygon.IsSelected)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                    if (adornerLayer != null)
                    {
                        adornerLayer.Remove(this);
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired !
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            Pen renderPen = new Pen(new SolidColorBrush(Colors.White), 0);
            foreach (var vertex in polygon.PolygonPoints)
            {
                var p = MilimetersToDpiHelper.ConvertToDpi(vertex);
                var radius = Math.Max(1.0, MilimetersToDpiHelper.ConvertToDpi(polygon.BorderWidth / 2));

                dc.DrawEllipse(Brushes.White, renderPen, p.ToPoint(), radius, radius);
            }

            // dc.DrawRectangle(Brushes.White, renderPen, GetTranslationRectangle());
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            e.Handled = false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangePolygonItem();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var b = base.MeasureOverride(constraint);
            InvalidateVisual();
            return b;
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            var g = Geometry.Empty;
            var radius = Math.Max(1.0, MilimetersToDpiHelper.ConvertToDpi(polygon.BorderWidth / 2) * 1.1);
            foreach (var p in polygon.PolygonPoints)
            {
                var vertex = MilimetersToDpiHelper.ConvertToDpi(p);

                g = Geometry.Combine(g, new EllipseGeometry(vertex.ToPoint(), radius, radius), GeometryCombineMode.Union, null);
            }

            return g;
        }

        //Rect GetTranslationRectangle()
        //{

        //    //calculate the center of gravity
        //    var middlePoint = new Point();
        //    foreach(var p in polygon.PolygonPoints)
        //    {
        //        middlePoint.Offset(p.X, p.Y);
        //    }
        //    middlePoint.X /= polygon.PolygonPoints.Count;
        //    middlePoint.Y /= polygon.PolygonPoints.Count;

        //    return new Rect(middlePoint.X - translateThumb.Width / 2,
        //                    middlePoint.Y - translateThumb.Height / 2,
        //                    translateThumb.Width,
        //                    translateThumb.Height);
        //}

        void ArrangePolygonItem()
        {
            if (polygon != null)
            {
                foreach (var thumb in vertexThumbs)
                {
                    var vertex = MilimetersToDpiHelper.ConvertToDpi(polygon.PolygonPoints[((int)thumb.Tag)]);
                    var width = Math.Max(1.0, MilimetersToDpiHelper.ConvertToDpi(polygon.BorderWidth / 2));
                    thumb.Width = thumb.Height = width;

                    var rect = new Rect(vertex.X - (width / 2), vertex.Y - (width / 2), width, width);

                    thumb.Arrange(rect);
                }

                //translateThumb.Arrange(GetTranslationRectangle());


                InvalidateVisual();
            }
        }
    }
}
