using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Core.Adorners
{
    public class TextAdorner : BaseCanvasItemAdorner
    {
        public TextAdorner(UIElement adornedElement) : base(adornedElement)
        {

            widthResizeThumb = new Thumb
            {
                Cursor = Cursors.SizeWE,
                Width = 4,
                Height = 4,
                Opacity = 0,
                Background = Brushes.Transparent,
            };

            widthResizeThumb.DragDelta += WidthResizeThumb_DragDelta;

            translateThumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Width = 4,
                Height = 4,
                Opacity = 0,
                Background = Brushes.Transparent,
            };

            translateThumb.DragDelta += translateThumb_DragDelta;

            visualChildren.Add(widthResizeThumb);
            visualChildren.Add(translateThumb);
            textItem = ((FrameworkElement)AdornedElement).DataContext as ITextCanvasItem;
            textItem.PropertyChanged += textItem_PropertyChanged;
        }



        Thumb widthResizeThumb;
        Thumb translateThumb;


        ITextCanvasItem textItem;


        private void textItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(textItem.IsSelected))
            {
                if (!textItem.IsSelected)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
                    if (adornerLayer != null)
                    {
                        adornerLayer.Remove(this);
                    }
                }
            }

            InvalidateVisual();
        }

        private void WidthResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (textItem != null)
            {
                var position = canvasModel.SnapToGridFromDpi(Mouse.GetPosition(AdornedElement).ToXPoint());

                var width = Math.Abs(textItem.X - position.X);
                textItem.Width = width;

                InvalidateVisual();
            }
        }


        private void translateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (textItem != null)
            {

                var newLocation = canvasModel.SnapToGridFromDpi(Mouse.GetPosition((IInputElement)VisualTreeHelper.GetParent(AdornedElement)).ToXPoint());

                textItem.X = newLocation.X;
                textItem.Y = newLocation.Y;

                InvalidateVisual();
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

            dc.DrawRectangle(Brushes.White, renderPen, GetResizeRectangle());
            dc.DrawRectangle(Brushes.White, renderPen, GetTranslationRectangle());
        }
        double GetSize()
        {
            //return Math.Max(0.2, imageItem.BorderWidth);
            return 1.0d;
        }
        Rect GetResizeRectangle()
        {
            var itemWidth = textItem.Width;
            if (double.IsNaN(itemWidth))
            {
                //todo measure the width from the text string
                itemWidth = MilimetersToDpiHelper.ConvertToMM(MeasureString(textItem.Text, textItem.FontFamily, textItem.FontSize).Width); //100;
            }

            //var margin = 0.0d;
            //var startRect = new Rect(itemWidth - widthResizeThumb.Width / 2 + margin, -widthResizeThumb.Height / 2, widthResizeThumb.Width, widthResizeThumb.Height);

            //return startRect;
            var size = GetSize();
            var startRect = new XRect(itemWidth - size / 2, -size / 2, size, size);
            startRect.X += textItem.X;
            startRect.Y += textItem.Y;

            return MilimetersToDpiHelper.ConvertToDpi(startRect).ToRect();
        }

        Size MeasureString(string text, string fontName, double fontSize)
        {
            var formattedText = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontName),
                fontSize,
                Brushes.Black,
                pixelsPerDip: 1.0d);

            return new Size(formattedText.Width, formattedText.Height);
        }

        Rect GetTranslationRectangle()
        {
            var size = GetSize();
            var r = new XRect(-size / 2, -size / 2, size, size);
            r.X += textItem.X;
            r.Y += textItem.Y;

            return MilimetersToDpiHelper.ConvertToDpi(r).ToRect();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            //to update position of thumbs
            ArrangeDesignerItem();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //to update position of thumbs
            ArrangeDesignerItem();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
           // e.Handled = false;
        }

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            Geometry g = new RectangleGeometry(GetResizeRectangle());
            g = Geometry.Combine(g, new RectangleGeometry(GetTranslationRectangle()), GeometryCombineMode.Union, null);
            return g;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeDesignerItem();

            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var b = base.MeasureOverride(constraint);
            InvalidateVisual();
            return b;
        }

        void ArrangeDesignerItem()
        {
            if (textItem != null)
            {
                var size = GetSize();
                widthResizeThumb.Width = widthResizeThumb.Height = size;
                translateThumb.Width = translateThumb.Height = size;

                widthResizeThumb.Arrange(GetResizeRectangle());

                translateThumb.Arrange(GetTranslationRectangle());
            }

            InvalidateVisual();
        }
    }
}
