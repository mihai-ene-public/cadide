using IDE.Core.Designers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IDE.Controls;

public class EllipseBoardShape : BoardShape
{
    #region Center

    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register(nameof(Center), typeof(Point), typeof(EllipseBoardShape),
            new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsRender));

    public Point Center
    {
        get { return (Point)GetValue(CenterProperty); }
        set { SetValue(CenterProperty, value); }
    }
    #endregion

    #region RadiusX

    public static readonly DependencyProperty RadiusXProperty =
        DependencyProperty.Register(nameof(RadiusX), typeof(double), typeof(EllipseBoardShape),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    [TypeConverter(typeof(LengthConverter))]
    public double RadiusX
    {
        get { return (double)GetValue(RadiusXProperty); }
        set { SetValue(RadiusXProperty, value); }
    }
    #endregion

    #region RadiusY

    public static readonly DependencyProperty RadiusYProperty =
        DependencyProperty.Register(nameof(RadiusY), typeof(double), typeof(EllipseBoardShape),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    [TypeConverter(typeof(LengthConverter))]
    public double RadiusY
    {
        get { return (double)GetValue(RadiusYProperty); }
        set { SetValue(RadiusYProperty, value); }
    }
    #endregion


    protected override Geometry DefiningGeometry
    {
        get
        {
            return new EllipseGeometry(Center, RadiusX, RadiusY);
        }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var penBrush = GetBrush(Stroke);
        var pen = new Pen(penBrush, StrokeThickness);


        Brush rectFillBush = null;
        if (IsFilled)
        {
            rectFillBush = GetBrush(Fill);
        }

        drawingContext.DrawEllipse(rectFillBush, pen, Center, RadiusX, RadiusY);
    }
}
