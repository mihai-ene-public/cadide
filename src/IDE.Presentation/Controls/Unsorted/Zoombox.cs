using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

namespace IDE.Core.Controls
{
    public class ZoomBox : Control
    {
       Thumb zoomThumb;
       Canvas zoomCanvas;
       Slider zoomSlider;
       ScaleTransform scaleTransform;

        #region DPs

        #region ScrollViewer
        public ScrollViewer ScrollViewer
        {
            get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
            set { SetValue(ScrollViewerProperty, value); }
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomBox));
        #endregion

        #region DesignerCanvas


        public static readonly DependencyProperty DesignerCanvasProperty =
            DependencyProperty.Register("DesignerCanvas", typeof(Canvas), typeof(ZoomBox),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnDesignerCanvasChanged)));


        public Canvas DesignerCanvas
        {
            get { return (Canvas)GetValue(DesignerCanvasProperty); }
            set { SetValue(DesignerCanvasProperty, value); }
        }


        static void OnDesignerCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (ZoomBox)d;
            var oldDesignerCanvas = (Canvas)e.OldValue;
            var newDesignerCanvas = target.DesignerCanvas;
            target.OnDesignerCanvasChanged(oldDesignerCanvas, newDesignerCanvas);
        }


        protected virtual void OnDesignerCanvasChanged(Canvas oldDesignerCanvas, Canvas newDesignerCanvas)
        {
            if (oldDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated -= DesignerCanvas_LayoutUpdated;
                newDesignerCanvas.MouseWheel -= DesignerCanvas_MouseWheel;
            }

            if (newDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated += DesignerCanvas_LayoutUpdated;
                newDesignerCanvas.MouseWheel += DesignerCanvas_MouseWheel;
                newDesignerCanvas.LayoutTransform = scaleTransform;
            }
        }

        #endregion

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ScrollViewer == null)
                return;

            zoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
            if (zoomThumb == null)
                throw new Exception("PART_ZoomThumb template is missing!");

            zoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;
            if (zoomCanvas == null)
                throw new Exception("PART_ZoomCanvas template is missing!");

            zoomSlider = Template.FindName("PART_ZoomSlider", this) as Slider;
            if (zoomSlider == null)
                throw new Exception("PART_ZoomSlider template is missing!");

            zoomThumb.DragDelta += Thumb_DragDelta;
            zoomSlider.ValueChanged +=ZoomSlider_ValueChanged;
            scaleTransform = new ScaleTransform();
        }

        void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            var scale = e.NewValue / e.OldValue;
            var halfViewportHeight = ScrollViewer.ViewportHeight / 2;
            var newVerticalOffset = ((ScrollViewer.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight);
            var halfViewportWidth = ScrollViewer.ViewportWidth / 2;
            var newHorizontalOffset = ((ScrollViewer.HorizontalOffset + halfViewportWidth) * scale - halfViewportWidth);
            scaleTransform.ScaleX *= scale;
            scaleTransform.ScaleY *= scale;
            ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }

        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double scale, xOffset, yOffset;
            InvalidateScale(out scale, out xOffset, out yOffset);
            ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + e.HorizontalChange / scale);
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + e.VerticalChange / scale);
        }

        void DesignerCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            double scale, xOffset, yOffset;
            InvalidateScale(out scale, out xOffset, out yOffset);
            zoomThumb.Width = ScrollViewer.ViewportWidth * scale;
            zoomThumb.Height = ScrollViewer.ViewportHeight * scale;
            Canvas.SetLeft(zoomThumb, xOffset + ScrollViewer.HorizontalOffset * scale);
            Canvas.SetTop(zoomThumb, yOffset + ScrollViewer.VerticalOffset * scale);
        }

        void DesignerCanvas_MouseWheel(object sender, EventArgs e)
        {
            var wheel = (MouseWheelEventArgs)e;

            //divide the value by 10 so that it is more smooth
            double value = Math.Max(0, wheel.Delta / 10);
            value = Math.Min(wheel.Delta / 12, 10);
            zoomSlider.Value += value;
        }

        void InvalidateScale(out double scale, out double xOffset, out double yOffset)
        {
            var w = DesignerCanvas.ActualWidth * scaleTransform.ScaleX;
            var h = DesignerCanvas.ActualHeight * scaleTransform.ScaleY;

            // zoom canvas size
            var x = zoomCanvas.ActualWidth;
            var y = zoomCanvas.ActualHeight;
            var scaleX = x / w;
            var scaleY = y / h;
            scale = (scaleX < scaleY) ? scaleX : scaleY;
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;
        }
    }
}
