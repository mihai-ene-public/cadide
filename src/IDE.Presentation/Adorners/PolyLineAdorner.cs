using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace IDE.Core.Adorners
{
    public class PolyLineAdorner : BaseCanvasItemAdorner
    {
        public PolyLineAdorner(UIElement adornedElement)
                : base(adornedElement)
        {
            vertexThumbs = new List<Thumb>();

            polyline = ((FrameworkElement)AdornedElement).DataContext as IPolylineCanvasItem;
            polyline.PropertyChanged += Polyline_PropertyChanged;

            for (int vertexIndex = 0; vertexIndex < polyline.Points.Count; vertexIndex++)
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
                thumb.PreviewMouseDown += Thumb_PreviewMouseDown;
                thumb.PreviewMouseUp += Thumb_PreviewMouseUp;

                visualChildren.Add(thumb);
                vertexThumbs.Add(thumb);
            }
        }


        List<Thumb> vertexThumbs;

        IPolylineCanvasItem polyline;
        XPoint? originalPosition;

        private void Thumb_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || canvasModel == null)
                return;
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var newPos = GetThumbPoint(thumb).Value;
            var oldPos = originalPosition.Value;
            originalPosition = null;

            canvasModel.RegisterUndoActionExecuted(
                undo: o =>
                {
                    SetThumbPoint(thumb, ref oldPos);
                    return null;
                },
                redo: o =>
                {
                    SetThumbPoint(thumb, ref newPos);
                    return null;
                },
                null
                );
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || canvasModel == null)
                return;

            if (originalPosition == null && sender is Thumb thumb)
                originalPosition = GetThumbPoint(thumb);
        }

        private XPoint? GetThumbPoint(Thumb thumb)
        {
            if (thumb == null)
                return null;

            var vIndex = (int)thumb.Tag;
            return polyline.Points[vIndex];
        }

        private void SetThumbPoint(Thumb thumb, ref XPoint point)
        {
            if (thumb == null)
                return;

            var vIndex = (int)thumb.Tag;
            polyline.Points[vIndex] = point;

            polyline.OnPropertyChanged(nameof(polyline.Points));
            polyline.OnPropertyChanged(nameof(ISegmentedPolylineSelectableCanvasItem.SelectedPoints));
            InvalidateVisual();
        }

        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

            var thumb = sender as Thumb;
            SetThumbPoint(thumb, ref position);
        }

        void Polyline_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(polyline.IsSelected))
            {
                if (!polyline.IsSelected)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                    if (adornerLayer != null)
                    {
                        adornerLayer.Remove(this);
                    }
                }
            }
            else
            {
                ArrangeItem();
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
            foreach (var vertex in polyline.Points)
            {
                var p = MilimetersToDpiHelper.ConvertToDpi(vertex);
                var radius = GetPolyRadiusDpi();

                dc.DrawEllipse(Brushes.White, renderPen, p.ToPoint(), radius, radius);
            }
        }

        double GetPolyRadiusDpi()
        {
            return MilimetersToDpiHelper.ConvertToDpi(polyline.Width * 0.5d);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            e.Handled = false;
        }

        private NetWireEndPointCollisionHelper _netWireEndPointCollisionHelper;
        private void EnsureNetWireCollisionHelper(NetWireCanvasItem netWire)
        {
            if (_netWireEndPointCollisionHelper == null)
            {
                var geometryHelper = ServiceProvider.Resolve<IGeometryOutlineHelper>();
                _netWireEndPointCollisionHelper = new NetWireEndPointCollisionHelper(netWire, (ISchematicDesigner)canvasModel, geometryHelper);
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (e.ChangedButton == MouseButton.Left && polyline is NetWireCanvasItem netWire)
            {
                var mousePos = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(this).ToXPoint());

                if (mousePos == netWire.StartPoint || mousePos == netWire.EndPoint)
                {
                    EnsureNetWireCollisionHelper(netWire);
                    _netWireEndPointCollisionHelper.HandleLinePoint(mousePos);
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeItem();

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

            var radius = GetPolyRadiusDpi();
            foreach (var p in polyline.Points)
            {
                var vertex = MilimetersToDpiHelper.ConvertToDpi(p);

                g = Geometry.Combine(g, new EllipseGeometry(vertex.ToPoint(), radius, radius), GeometryCombineMode.Union, null);
            }

            return g;
        }


        void ArrangeItem()
        {
            if (polyline != null)
            {
                foreach (var thumb in vertexThumbs)
                {
                    var vertex = MilimetersToDpiHelper.ConvertToDpi(polyline.Points[((int)thumb.Tag)]);

                    var radius = GetPolyRadiusDpi();
                    var width = 2.0 * radius;
                    thumb.Width = thumb.Height = 2.0 * radius;

                    var rect = new Rect(vertex.X - radius, vertex.Y - radius, width, width);

                    thumb.Arrange(rect);
                }

                InvalidateVisual();
            }
        }
    }
}
