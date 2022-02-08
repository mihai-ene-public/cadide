using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System;
using IDE.Core.Designers;
using IDE.Core.Controls;
using IDE.Core.Converters;
using IDE.Core.Units;
using IDE.Core.Interfaces;

namespace IDE.Core.Adorners
{
    public class MeasureDistance2DAdorner : Adorner
    {
        Point? startPoint;
        Point? endPoint;
        Pen rubberbandPen;

        FrameworkElement designerCanvas;
        IDrawingViewModel canvasModel;
        MillimeterUnit mmUnit = new MillimeterUnit();
        AbstractUnit gridUnit;

        public MeasureDistance2DAdorner(FrameworkElement canvas, Point? dragStartPoint)
            : base(canvas)
        {
            designerCanvas = canvas;
            startPoint = dragStartPoint;
            var t = GetPenThickness();
            rubberbandPen = new Pen(Brushes.LightGray, t);
            rubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);

            canvasModel = designerCanvas.DataContext as IDrawingViewModel;

            //a copy of grid unit
            var cg = canvasModel.CanvasGrid as CanvasGrid;
            gridUnit = (AbstractUnit)Activator.CreateInstance(cg.GridSizeModel.SelectedItem.GetType());


        }

        double GetPenThickness()
        {
            var thickness = 1.0d;
            var drawingCanvas = AdornedElement as DrawingCanvas;
            if (drawingCanvas != null)
                thickness = thickness / drawingCanvas.Scale;

            return thickness;
        }

        double GetUnscaledValue(double value)
        {
            var drawingCanvas = AdornedElement as DrawingCanvas;
            if (drawingCanvas != null)
                value = value / drawingCanvas.Scale;

            return value;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (!IsMouseCaptured)
                    CaptureMouse();

                endPoint = e.GetPosition(this);
                InvalidateVisual();
            }
            else
            {
                if (IsMouseCaptured)
                    ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            // release mouse capture
            if (IsMouseCaptured)
                ReleaseMouseCapture();

            // remove this adorner from adorner layer
            var adornerLayer = AdornerLayer.GetAdornerLayer(designerCanvas);
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            //var vm = designerCanvas.DataContext as IDrawingViewModel;
            //vm?.UpdateSelection();

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired !
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (startPoint.HasValue && endPoint.HasValue)
            //dc.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(startPoint.Value, endPoint.Value));
            {
                dc.DrawLine(rubberbandPen, startPoint.Value, endPoint.Value);

                //show text
                var lenDPI = (endPoint.Value - startPoint.Value).Length;
                var lenMM = MilimetersToDpiHelper.ConvertToMM(lenDPI);
                mmUnit.CurrentValue = lenMM;
                var crtDistance = gridUnit.ConvertFrom(mmUnit);
                var textPos = endPoint.Value + new Vector(5, -20) / canvasModel.Scale;

                var tf = new Typeface("Segoe UI");
                var formattedText = new FormattedText(
                    $"{crtDistance:0.0000} {gridUnit.DisplayNameShort}",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    tf,
                    GetUnscaledValue(18),
                    Brushes.White,
                    pixelsPerDip: 1.0d);
                dc.DrawText(formattedText, textPos);
            }
        }
    }
}
