using IDE.Core.Designers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Controls
{
    public class RectangleBoardShape : BoardShape
    {
        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(RectangleBoardShape),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        [TypeConverter(typeof(LengthConverter))]
        public double CornerRadius
        {
            get { return (double)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion

        protected override Geometry DefiningGeometry
        {
            get
            {
                var cornerRadius = CornerRadius;
                var rect = GetRect();

                return new RectangleGeometry(rect, cornerRadius, cornerRadius);
            }
        }

        private Rect GetRect()
        {
            var width = Width;
            var height = Height;

            var rect = new Rect(-0.5 * width, -0.5 * height, width, height);

            return rect;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var penBrush = GetBrush(Stroke);
            var pen = new Pen(penBrush, StrokeThickness);

            var rect = GetRect();
            var cornerRadius = CornerRadius;

            Brush rectFillBush = null;
            if (IsFilled)
            {
                rectFillBush = GetBrush(Fill);
            }

            drawingContext.DrawRoundedRectangle(rectFillBush, pen, rect, cornerRadius, cornerRadius);
        }
    }
}
