using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Eagle;
using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;

namespace IDE.Controls;

public class TrackBoardShape : BoardShape
{
    #region Points

    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register(nameof(Points), typeof(PointCollection), typeof(TrackBoardShape),
            new FrameworkPropertyMetadata(new PointCollection(), FrameworkPropertyMetadataOptions.AffectsRender));

    public PointCollection Points
    {
        get { return (PointCollection)GetValue(PointsProperty); }
        set { SetValue(PointsProperty, value); }
    }
    #endregion

    #region SelectedPoints
    public static readonly DependencyProperty SelectedPointsProperty =
        DependencyProperty.Register(nameof(SelectedPoints), typeof(PointCollection), typeof(TrackBoardShape),
            new FrameworkPropertyMetadata(new PointCollection(), FrameworkPropertyMetadataOptions.AffectsRender));

    public PointCollection SelectedPoints
    {
        get { return (PointCollection)GetValue(SelectedPointsProperty); }
        set { SetValue(SelectedPointsProperty, value); }
    }
    #endregion

    #region SelectedPointsBrush
    public static readonly DependencyProperty SelectedPointsBrushProperty =
               DependencyProperty.Register(
                       nameof(SelectedPointsBrush),
                       typeof(Brush),
                       typeof(TrackBoardShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public Brush SelectedPointsBrush
    {
        get { return (Brush)GetValue(SelectedPointsBrushProperty); }
        set { SetValue(SelectedPointsBrushProperty, value); }
    }
    #endregion

    #region IsFaulty
    public static readonly DependencyProperty IsFaultyProperty =
    DependencyProperty.Register(
            nameof(IsFaulty),
            typeof(bool),
            typeof(TrackBoardShape),
            new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public bool IsFaulty
    {
        get { return (bool)GetValue(IsFaultyProperty); }
        set { SetValue(IsFaultyProperty, value); }
    }
    #endregion

    #region FaultyBrush
    public static readonly DependencyProperty FaultyBrushProperty =
               DependencyProperty.Register(
                       nameof(FaultyBrush),
                       typeof(Brush),
                       typeof(TrackBoardShape),
                       new FrameworkPropertyMetadata(
                               (Brush)null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public Brush FaultyBrush
    {
        get { return (Brush)GetValue(FaultyBrushProperty); }
        set { SetValue(FaultyBrushProperty, value); }
    }
    #endregion

    #region SignalName
    public static readonly DependencyProperty SignalNameProperty =
               DependencyProperty.Register(
                       nameof(SignalName),
                       typeof(string),
                       typeof(TrackBoardShape),
                       new FrameworkPropertyMetadata(
                               null,
                               FrameworkPropertyMetadataOptions.AffectsRender |
                               FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

    public string SignalName
    {
        get { return (string)GetValue(SignalNameProperty); }
        set { SetValue(SignalNameProperty, value); }
    }
    #endregion

    protected override Geometry DefiningGeometry
    {
        get
        {
            return GetDefiningGeometry();
        }
    }

    private Geometry GetDefiningGeometry()
    {
        var pointCollection = Points;
        var pathFigure = new PathFigure();

        if (pointCollection == null)
        {
            return Geometry.Empty;
        }

        if (pointCollection.Count > 0)
        {
            pathFigure.StartPoint = pointCollection[0];

            if (pointCollection.Count > 1)
            {
                var array = new Point[pointCollection.Count - 1];

                for (int i = 1; i < pointCollection.Count; i++)
                {
                    array[i - 1] = pointCollection[i];
                }

                pathFigure.Segments.Add(new PolyLineSegment(array, true));
            }
        }

        var polylineGeometry = new PathGeometry();
        polylineGeometry.Figures.Add(pathFigure);


        if (polylineGeometry.Bounds == Rect.Empty)
        {
            return Geometry.Empty;
        }
        else
        {
            return polylineGeometry;
        }
    }

    private Pen GetRoundPen(Brush brush, double penThickness)
    {
        var pen = new Pen(brush, penThickness);
        pen.StartLineCap = PenLineCap.Round;
        pen.EndLineCap = PenLineCap.Round;
        pen.LineJoin = PenLineJoin.Round;
        pen.DashCap = PenLineCap.Round;

        return pen;
    }
    protected override void OnRender(DrawingContext drawingContext)
    {
        var points = Points;
        if (points?.Count < 2)
        {
            return;
        }

        var trackWidth = StrokeThickness;
        var brush = IsFaulty ? FaultyBrush : Stroke;
        var penBrush = GetBrush(brush);
        var pen = GetRoundPen(penBrush, trackWidth);

        //track
        for (int i = 0; i < points.Count - 1; i++)
        {
            drawingContext.DrawLine(pen, points[i], points[i + 1]);
        }

        //selected portion
        var selectedPoints = SelectedPoints;
        if (selectedPoints?.Count > 1)
        {
            var selectedPenBrush = GetBrush(SelectedPointsBrush);
            var selectedPen = GetRoundPen(selectedPenBrush, trackWidth);

            for (int i = 0; i < selectedPoints.Count - 1; i++)
            {
                drawingContext.DrawLine(selectedPen, selectedPoints[i], selectedPoints[i + 1]);
            }
        }

        //signal name
        var signalName = SignalName;
        if (!string.IsNullOrEmpty(signalName))
        {
            var tf = new Typeface("Segoe UI");
            var whiteBrush = new SolidColorBrush(Colors.White);
            var pixelsPerDip = 1.0d;// 96/96


            for (int i = 0; i < points.Count - 1; i++)
            {
                var sp = points[i];
                var ep = points[i + 1];

                //a rect that would have the segment horizontal with rect width being the segment length
                var w = (ep - sp).Length;
                var h = trackWidth;
                var fontSize = GetFontSize(signalName, new XSize(w, h));

                if (fontSize < 0.01d)
                    continue;

                var sizeText = GetSizeForPadText(signalName, fontSize);
                var pos = GetSignalNamePosition(sp, ep, fontSize, sizeText);

                var ft = new FormattedText(signalName, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, tf, fontSize, whiteBrush, pixelsPerDip);
                ft.TextAlignment = TextAlignment.Center;

                var textGeometry = ft.BuildGeometry(new Point(0, 0));
                var tg = new TransformGroup();
                tg.Children.Add(new RotateTransform(pos.Rotation));
                tg.Children.Add(new TranslateTransform(pos.X, pos.Y));
                textGeometry.Transform = tg;

                drawingContext.DrawGeometry(whiteBrush, null, textGeometry);
            }
        }

    }

    private double GetFontSize(string text, XSize availableSize)
    {
        var sizeFactor = 0.8d;
        var sLen = 1.0d;
        if (!string.IsNullOrEmpty(text))
            sLen = text.Length;

        var fontSize = sizeFactor * availableSize.Height;
        var textLen = fontSize * sLen;
        if (textLen > availableSize.Width)
        {
            var scale = availableSize.Width / textLen;
            fontSize *= scale;
        }

        return fontSize;
    }
    private XSize GetSizeForPadText(string text, double fontSize)
    {
        var sLen = 1.0d;
        if (!string.IsNullOrEmpty(text))
            sLen = text.Length;


        var baseFontSize = 0.8;
        var left = 0.08 * fontSize / baseFontSize;
        var right = 0.08 * fontSize / baseFontSize;
        var top = 0.3 * fontSize / baseFontSize;
        var bottom = 0.18 * fontSize / baseFontSize;

        var w = sLen * (fontSize + left + right);
        var h = fontSize + top + bottom;

        //wpf rounds off to 1/96 points for width and height
        //so we must convert to dpi, round the value and then back to mm
        var dotsWidth = Math.Round(MilimetersToDpiHelper.ConvertToDpi(w));
        var dotsHeight = Math.Round(MilimetersToDpiHelper.ConvertToDpi(h));
        if (dotsWidth < 1.00d)
            dotsWidth = 1.00d;
        if (dotsHeight < 1.00d)
            dotsHeight = 1.00d;
        w = MilimetersToDpiHelper.ConvertToMM(dotsWidth);
        h = MilimetersToDpiHelper.ConvertToMM(dotsHeight);

        return new XSize(w, h);
    }

    private IPosition GetSignalNamePosition(Point sp, Point ep, double fontSize, XSize sizeText)
    {
        var newRot = 0.0d;

        if (sp.X == ep.X)
        {
            newRot = -90;
        }

        else if (sp.Y == ep.Y)
        {
            newRot = 0.0d;
        }
        else
        {
            var rads = Math.Atan2(ep.Y - sp.Y, ep.X - sp.X);
            newRot = rads * 180 / Math.PI;
        }

        if (newRot > 90.0d)
            newRot -= 180.0;
        if (newRot < -90.0d)
            newRot += 180;

        var newPos = new XPoint((0.5 * (sp.X + ep.X - 0.5 * sizeText.Width)), (0.5 * (sp.Y + ep.Y)) - 0.75 * fontSize);

        var tg = new XTransformGroup();
        var mp = new XPoint(0.5 * (sp.X + ep.X), 0.5 * (sp.Y + ep.Y));
        var rotateTransform = new XRotateTransform(newRot)
        {
            CenterX = mp.X,
            CenterY = mp.Y
        };

        tg.Children.Add(rotateTransform);
        newPos = tg.Value.Transform(newPos);

        return new PositionData
        {
            X = newPos.X,
            Y = newPos.Y,
            Rotation = newRot
        };
    }

}
