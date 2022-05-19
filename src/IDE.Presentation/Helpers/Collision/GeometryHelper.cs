using IDE.Core.Converters;
using IDE.Core.Designers;
using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Model.GlobalRepresentation.Primitives;
using IDE.Core.Presentation;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IDE.Core.Collision
{
    //this should be on the model layer, but since we have Geometry class which is WPF and DirectX Windows,we throw it on the presentation

    public class GeometryHelper : IGeometryHelper
    {
        Pen thinPen = new Pen(Brushes.Transparent, 0.01);

        const double defaultTolerance = 0.01;//5e-3;

        public XRect GetGeometryBounds(ICanvasItem item)
        {
            var g = GetGeometryInternal(item);
            return g.Bounds.ToXRect();
        }

        public XRect GetGeometryBounds(object geometry)
        {
            var g = (Geometry)geometry;
            return g.Bounds.ToXRect();
        }

        public Geometry GetLineGeometry(ILineCanvasItem line, double tolerance = defaultTolerance)
        {
            var lg = new LineGeometry(new Point(line.X1, line.Y1), new Point(line.X2, line.Y2));

            var pen = new Pen(Brushes.Transparent, line.Width)
            {
                LineJoin = PenLineJoin.Round,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            var p = lg.GetWidenedPathGeometry(pen, tolerance, ToleranceType.Absolute);
            ////start line cap
            //p = Geometry.Combine(p, new EllipseGeometry(new Point(line.X1, line.Y1), line.Width / 2, line.Width / 2), GeometryCombineMode.Union, null, 1e-3, ToleranceType.Absolute);
            ////end line cap
            //p = Geometry.Combine(p, new EllipseGeometry(new Point(line.X2, line.Y2), line.Width / 2, line.Width / 2), GeometryCombineMode.Union, null, 1e-3, ToleranceType.Absolute);
            return p;
        }

        public Geometry GetPolylineGeometry(IPolylineCanvasItem track, double tolerance = defaultTolerance)
        {
            var pen = new Pen(Brushes.Transparent, track.Width)
            {
                LineJoin = PenLineJoin.Round,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };

            var p = new PathGeometry();
            if (track.Points.Count > 0)
            {
                var start = track.Points[0].ToPoint();
                var segments = new List<PathSegment>();
                for (int i = 1; i < track.Points.Count; i++)
                {
                    segments.Add(new LineSegment(track.Points[i].ToPoint(), true) { IsSmoothJoin = true });
                }

                var figure = new PathFigure(start, segments, false);
                figure.IsFilled = false;
                p.Figures.Add(figure);
            }

            //p=p.GetOutlinedPathGeometry(tolerance,ToleranceType.Absolute)

            var g = p.GetWidenedPathGeometry(pen,
                                             tolerance,//0.001,
                                             ToleranceType.Absolute)
                     .GetOutlinedPathGeometry(tolerance, ToleranceType.Absolute);

            return g;
        }

        public Geometry GetEllipseGeometry(IJunctionCanvasItem junction)
        {
            //return new EllipseGeometry(new Point(junction.X, junction.Y), junction.Diameter / 2, junction.Diameter / 2);
            return new EllipseGeometry(new Point(), junction.Diameter / 2, junction.Diameter / 2);
        }

        public Geometry GetEllipseGeometry(ICircleCanvasItem circle, double tolerance = defaultTolerance)
        {
            var dext = circle.Diameter + Math.Abs(circle.BorderWidth);
            var dint = circle.Diameter - Math.Abs(circle.BorderWidth);
            var center = new Point();
            Geometry g = new EllipseGeometry(center, dext / 2, dext / 2);

            if (!circle.IsFilled)
            {
                g = Geometry.Combine(g, new EllipseGeometry(center, dint / 2, dint / 2), GeometryCombineMode.Exclude, null, tolerance, ToleranceType.Absolute);
            }
            return g;
        }

        public Geometry GetHoleGeometry(IHoleCanvasItem hole)
        {
            if (hole.DrillType == DrillType.Drill)
                return new EllipseGeometry(new Point()//hole.X, hole.Y)
                                                    , hole.Drill / 2, hole.Drill / 2);

            var t = new TransformGroup();
            t.Children.Add(new RotateTransform(hole.Rot));
            t.Children.Add(new TranslateTransform(hole.X, hole.Y));
            var posX = -0.5 * hole.Drill;
            var posY = -0.5 * hole.Height;
            return new RectangleGeometry(new Rect(posX, posY, hole.Drill, hole.Height), 0.5 * hole.Drill, 0.5 * hole.Drill, t);
        }

        public Geometry GetEllipseGeometry(IViaCanvasItem via)
        {
            return new EllipseGeometry(new Point()
                                       , 0.5 * via.Diameter, 0.5 * via.Diameter);
        }

        public Geometry GetPinGeometry(IPinCanvasItem pin, double tolerance = defaultTolerance)
        {
            //the geometry will be oriented by the transform which takes into account the orientation

            var startPoint = new Point();// new Point(pin.X, pin.Y);
            var endPoint = new Point(pin.PinLength, 0); //new Point(startPoint.X + pin.PinLength, startPoint.Y);

            var lg = new LineGeometry(startPoint, endPoint);

            var pen = new Pen(Brushes.Transparent, pin.Width)
            {
                LineJoin = PenLineJoin.Round,
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            var p = lg.GetWidenedPathGeometry(pen, tolerance, ToleranceType.Absolute);

            return p;
        }

        public Geometry GetRectangleGeometry(ILabelCanvasItem netLabel)
        {
            // small rectangle arround the mouse (this it what has to be clicked on a net wire)
            return new RectangleGeometry(new Rect(netLabel.X, netLabel.Y, 10, 10));
        }

        public Geometry GetRectangleGeometry(IRectangleCanvasItem rectangle)
        {
            var border = Math.Abs(rectangle.BorderWidth);

            //var t = new TransformGroup();
            //t.Children.Add(new RotateTransform(rectangle.Rot));
            //t.Children.Add(new TranslateTransform(rectangle.X, rectangle.Y));
            var posX = -0.5 * rectangle.Width;
            var posY = -0.5 * rectangle.Height;
            Geometry g = new RectangleGeometry(new Rect(posX - border / 2,
                                                        posY - border / 2,
                                                        rectangle.Width + border,
                                                        rectangle.Height + border),
                                               rectangle.CornerRadius,
                                               rectangle.CornerRadius);
            if (!rectangle.IsFilled)
            {
                g = Geometry.Combine(g, new RectangleGeometry(new Rect(posX + border / 2,
                                                                       posY + border / 2,
                                                                       rectangle.Width - border,
                                                                       rectangle.Height - border),
                                                              rectangle.CornerRadius,
                                                              rectangle.CornerRadius), GeometryCombineMode.Exclude, null);
            }
            return g;
        }

        //not generated properly (no fill, no round corners)
        public Geometry GetPolygonGeometry(IPolygonCanvasItem poly, double tolerance = defaultTolerance)
        {

            var g = new PathGeometry();

            if (poly.PolygonPoints.Count > 0)
            {
                var start = poly.PolygonPoints[0].ToPoint();
                var segments = new List<LineSegment>();
                for (int i = 1; i < poly.PolygonPoints.Count; i++)
                {
                    segments.Add(new LineSegment(poly.PolygonPoints[i].ToPoint(), true) { IsSmoothJoin = true });
                }

                var figure = new PathFigure(start, segments, true);
                figure.IsFilled = poly.IsFilled;
                g.Figures.Add(figure);
            }

            //we comment this below because the clearance is shown with added border value;
            //see if this affects other things
            //if (poly.BorderWidth > 0)
            //{
            //    var penClearance = new Pen(Brushes.Transparent, poly.BorderWidth)
            //    {
            //        LineJoin = PenLineJoin.Round,
            //        StartLineCap = PenLineCap.Round,
            //        EndLineCap = PenLineCap.Round
            //    };

            //    g = Geometry.Combine(g, g.GetWidenedPathGeometry(penClearance, tolerance, ToleranceType.Absolute),
            //                         GeometryCombineMode.Union, null, tolerance, ToleranceType.Absolute);
            //}

            return g;
        }

        //fill is not working
        public Geometry GetArcGeometry(IArcCanvasItem arc, double tolerance = defaultTolerance)
        {
            var p = new PathGeometry();
            var figure = new PathFigure
            {
                IsClosed = arc.IsFilled,
                IsFilled = arc.IsFilled,
                StartPoint = new Point(arc.StartPointX, arc.StartPointY)
            };
            figure.Segments.Add(new ArcSegment
            {
                Point = new Point(arc.EndPointX, arc.EndPointY),
                IsStroked = true,
                IsSmoothJoin = true,
                RotationAngle = arc.RotationAngle,
                IsLargeArc = arc.IsLargeArc,
                Size = arc.Size.ToSize(),
                SweepDirection = arc.SweepDirection.ToSweepDirection()
            });
            p.Figures.Add(figure);

            var g = p.GetWidenedPathGeometry(new Pen(Brushes.Transparent, Math.Max(0.01, arc.BorderWidth)), 1e-3, ToleranceType.Absolute);
            //var g = p.GetOutlinedPathGeometry(1e-3, ToleranceType.Absolute);

            //start line cap
            g = Geometry.Combine(g, new EllipseGeometry(figure.StartPoint, arc.BorderWidth / 2, arc.BorderWidth / 2), GeometryCombineMode.Union, null, tolerance, ToleranceType.Absolute);
            //end line cap
            g = Geometry.Combine(g, new EllipseGeometry(new Point(arc.EndPointX, arc.EndPointY), arc.BorderWidth / 2, arc.BorderWidth / 2), GeometryCombineMode.Union, null, tolerance, ToleranceType.Absolute);
            return g;

            // return p;
        }

        //this is tricky: position is not right and size/scale is not properly (fontsize in points, we need mm)
        public Geometry GetTextGeometry(ITextCanvasItem text)
        {
            return GetTextGeometry(text.Text, text.FontFamily, text.FontSize, text.Bold, text.Italic);

            //var fontStyle = FontStyles.Normal;
            //if (text.Italic)
            //    fontStyle = FontStyles.Italic;
            //var fontWeight = FontWeights.Normal;
            //if (text.Bold) fontWeight = FontWeights.Bold;

            //var fontSizeMM = 0.0254 * 1000 * text.FontSize / 96.0d;

            //var tf = new Typeface(new FontFamily(text.FontFamily), fontStyle, fontWeight, FontStretches.Normal);
            //var formattedText = new FormattedText(
            //    text.Text,
            //    System.Globalization.CultureInfo.CurrentCulture,
            //    FlowDirection.LeftToRight,
            //    tf,
            //    fontSizeMM,
            //    Brushes.Black,
            //    pixelsPerDip: 1.0d);


            //var origin = new Point();

            //var textGeometry = formattedText.BuildGeometry(origin);
            //return textGeometry;
        }

        private Geometry GetTextGeometry(string textString, string fontFamily, double fontSize, bool bold, bool italic)
        {
            var fontStyle = FontStyles.Normal;
            if (italic)
                fontStyle = FontStyles.Italic;

            var fontWeight = FontWeights.Normal;
            if (bold)
                fontWeight = FontWeights.Bold;

            var fontSizeMM = 0.0254 * 1000 * fontSize / 96.0d;

            var tf = new Typeface(new FontFamily(fontFamily), fontStyle, fontWeight, FontStretches.Normal);
            var formattedText = new FormattedText(
                textString,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                tf,
                fontSizeMM,
                Brushes.Black,
                pixelsPerDip: 1.0d);


            var origin = new Point();

            var textGeometry = formattedText.BuildGeometry(origin);
            return textGeometry;
        }

        public void GetTextOutlines(ITextCanvasItem text, List<List<XPoint[]>> outlines)
        {
            var textGeometry = GetTextGeometry(text);
            AppendOutlinesInternal(textGeometry, outlines);
        }

        public void GetTextOutlines(GlobalTextPrimitive text, List<List<XPoint[]>> outlines)
        {
            var textGeometry = GetTextGeometry(text.Text, text.FontFamily, text.FontSize, text.Bold, text.Italic);
            AppendOutlinesInternal(textGeometry, outlines);
        }

        public void AppendOutlinesInternal(Geometry geometry, List<List<XPoint[]>> outlines, double tolerance = defaultTolerance)
        {
            var group = geometry as GeometryGroup;
            if (group != null)
            {
                foreach (var g in group.Children)
                {
                    AppendOutlinesInternal(g, outlines);
                }

                return;
            }

            //var pathGeometry = (geometry is PathGeometry) ? geometry as PathGeometry : geometry.GetOutlinedPathGeometry();
            var pathGeometry = geometry.GetFlattenedPathGeometry(tolerance, ToleranceType.Absolute);
            if (pathGeometry != null)
            {
                var figures = pathGeometry.Figures.Select(figure => ToPolyLine(figure)).ToList();
                outlines.Add(figures);
                return;
            }

            throw new NotImplementedException();
        }

        public void AppendOutlines(object geometry, List<List<XPoint[]>> outlines)
        {
            AppendOutlinesInternal((Geometry)geometry, outlines);
        }

        public Geometry GetTextGeometry(ITextMonoLineCanvasItem text, double tolerance = defaultTolerance)
        {
            //var fp = (text as ISelectableItem).ParentObject as IFootprintBoardCanvasItem;
            var textGeom = Geometry.Empty;
            //var t = new TransformGroup();
            //t.Children.Add(new RotateTransform(text.Rot));
            //t.Children.Add(new TranslateTransform(text.X, text.Y));

            var x = 0.0d;
            foreach (var letter in text.LetterItems)
            {

                foreach (var item in letter.Items)
                {
                    var gItem = GetGeometryInternal(item);
                    var lt = new TransformGroup();

                    //if (fp != null && fp.Placement == FootprintPlacement.Bottom)
                    //    lt.Children.Add(new ScaleTransform { ScaleX = -1 });
                    lt.Children.Add(new TranslateTransform(x, 0));

                    gItem.Transform = lt;

                    textGeom = Geometry.Combine(textGeom, gItem, GeometryCombineMode.Union, null, tolerance, ToleranceType.Absolute);
                }

                x += letter.FontSize;
            }

            // textGeom.Transform = t;
            return textGeom;
        }

        public Geometry GetPadGeometry(IPadCanvasItem pad, double tolerance = defaultTolerance)
        {
            if (pad == null)
                return Geometry.Empty;

            var fp = ( pad as ISelectableItem ).ParentObject as IFootprintBoardCanvasItem;

            var padRot = pad.Rot;

            var posX = pad.X - pad.Width * 0.5;
            var posY = pad.Y - pad.Height * 0.5;
            Geometry rg = new RectangleGeometry(new Rect(-0.5 * pad.Width, -0.5 * pad.Height, pad.Width, pad.Height), pad.CornerRadius, pad.CornerRadius, null);

            return rg;
        }

        public object GetRegionGeometry(IRegionCanvasItem region)
        {
            var g = new PathGeometry();
            var figure = new PathFigure { StartPoint = region.StartPoint.ToPoint() };
            g.Figures.Add(figure);

            foreach (var item in region.Items)
            {
                PathSegment segment = null;
                if (item is ILineRegionItem)
                {
                    var lineItem = item as ILineRegionItem;

                    segment = new LineSegment(lineItem.EndPoint.ToPoint(), true)
                    {
                        IsSmoothJoin = true
                    };
                }
                else if (item is IArcRegionItem)
                {
                    var arcItem = item as IArcRegionItem;

                    segment = new ArcSegment
                    {
                        Point = arcItem.EndPoint.ToPoint(),
                        IsStroked = true,
                        IsSmoothJoin = true,
                        IsLargeArc = arcItem.IsLargeArc,
                        SweepDirection = arcItem.SweepDirection.ToSweepDirection(),
                        Size = new Size(arcItem.SizeDiameter, arcItem.SizeDiameter)
                    };
                }

                if (segment != null)
                    figure.Segments.Add(segment);
            }

            return g;
        }

        public Geometry GetPlaneGeometry(IPlaneBoardCanvasItem plane)
        {
            try
            {
                dynamic brdItem = plane;
                var brd = brdItem.LayerDocument as IBoardDesigner;
                if (brd != null)
                {
                    return GetRegionGeometry(brd.BoardOutline) as Geometry;
                }
            }
            catch
            {
            }
            return Geometry.Empty;
        }


        public object GetGeometry(ICanvasItem item, double tolerance = defaultTolerance, bool applyTransform = false)
        {
            var g = GetGeometryInternal(item, tolerance);
            if (applyTransform && !g.IsEmpty() && item is ISelectableItem s)
            {
                g.Transform = s.GetTransform().ToMatrixTransform();
            }

            return g;
        }

        internal Geometry GetGeometryInternal(ICanvasItem item, double tolerance = defaultTolerance)
        {
            if (item != null)
            {
                if (item is ILineCanvasItem)
                    return GetLineGeometry((ILineCanvasItem)item);
                else if (item is IPolylineCanvasItem)
                    return GetPolylineGeometry((IPolylineCanvasItem)item, tolerance: tolerance);
                else if (item is IRectangleCanvasItem)
                    return GetRectangleGeometry((IRectangleCanvasItem)item);
                else if (item is IJunctionCanvasItem)
                    return GetEllipseGeometry((IJunctionCanvasItem)item);
                else if (item is ICircleCanvasItem)
                    return GetEllipseGeometry((ICircleCanvasItem)item);
                else if (item is IHoleCanvasItem)
                    return GetHoleGeometry((IHoleCanvasItem)item);
                else if (item is IViaCanvasItem)
                    return GetEllipseGeometry((IViaCanvasItem)item);
                else if (item is IPinCanvasItem)
                    return GetPinGeometry((IPinCanvasItem)item);
                else if (item is ILabelCanvasItem)
                    return GetRectangleGeometry((ILabelCanvasItem)item);
                else if (item is IPadCanvasItem)
                    return GetPadGeometry((IPadCanvasItem)item, tolerance: tolerance);
                //else if (item is IPadRefDesignerItem)
                //    return GetPadGeometry((item as IPadRefDesignerItem).PadDesignerItem);
                else if (item is IPolygonCanvasItem)
                    return GetPolygonGeometry((IPolygonCanvasItem)item);
                else if (item is IArcCanvasItem)
                    return GetArcGeometry((IArcCanvasItem)item);
                else if (item is ITextCanvasItem)
                    return GetTextGeometry((ITextCanvasItem)item);
                else if (item is ITextMonoLineCanvasItem)
                    return GetTextGeometry((ITextMonoLineCanvasItem)item);
                else if (item is IRegionCanvasItem)
                    return (Geometry)GetRegionGeometry((IRegionCanvasItem)item);
                else if (item is IPlaneBoardCanvasItem plane)
                    return GetPlaneGeometry(plane);
            }


            //we want to have an empty geometry by default
            return Geometry.Empty;//new PathGeometry();

        }



        public Geometry GetIntersection(ICanvasItem item1, ICanvasItem item2)
        {
            var geometry1 = GetGeometryInternal(item1);
            var geometry2 = GetGeometryInternal(item2);

            var intersection = Geometry.Combine(geometry1, geometry2, GeometryCombineMode.Intersect, null);
            return intersection;
        }

        public Geometry GetIntersectionInternal(Geometry geometry, ICanvasItem item)
        {
            var geometry2 = GetGeometryInternal(item);

            var intersection = Geometry.Combine(geometry, geometry2, GeometryCombineMode.Intersect, null);
            return intersection;
        }

        public object GetIntersection(object geometry, ICanvasItem item)
        {
            return GetIntersectionInternal((Geometry)geometry, item);
        }

        internal Geometry GetIntersectionInternal(Geometry geometry1, Geometry geometry2)
        {
            var intersection = Geometry.Combine(geometry1, geometry2, GeometryCombineMode.Intersect, null);
            return intersection;
        }

        public object GetIntersection(object geometry1, object geometry2)
        {
            return GetIntersectionInternal((Geometry)geometry1, (Geometry)geometry2);
        }

        public bool ItemIntersectsPoint(ICanvasItem item, XPoint point, double pointDiameter)
        {
            var pointGeom = new EllipseGeometry(point.ToPoint(), pointDiameter / 2, pointDiameter / 2);
            return Intersects(pointGeom, item);
        }

        public bool ItemIntersectsRectangle(ICanvasItem item, XRect rect)
        {
            var itemBounds = ( (ISelectableItem)item ).GetBoundingRectangle();

            if (rect.IntersectsWith(itemBounds))
            {
                var geometry2 = GetGeometryInternal(item);
                if (geometry2.IsEmpty())
                    return true;

                var rectGeom = new RectangleGeometry(rect.ToRect());
                geometry2.Transform = ( (ISelectableItem)item ).GetTransform().ToMatrixTransform();

                return Intersects(rectGeom, geometry2);
            }

            return false;
        }

        public bool Intersects(Geometry geometry1, Geometry geometry2, double tolerance = defaultTolerance)
        {
            if (geometry1.IsEmpty() || geometry2.IsEmpty())
                return false;

            //todo: to improve the geometry collision we must have a Geometry on ICanvasItem, we refresh this as needed

            //if the bounds intersect check in more detail
            //a geometry.Bounds doesnt take into account its transform
            //if (geometry1.Bounds.Intersects(geometry2.Bounds))
            {
                var detail = geometry1.FillContainsWithDetail(geometry2, tolerance, ToleranceType.Absolute);
                return detail != IntersectionDetail.Empty && detail != IntersectionDetail.NotCalculated;
            }

            //return false;
        }

        public bool Intersects(object geometry1, object geometry2)
        {
            var g1 = geometry1 as Geometry;
            var g2 = geometry2 as Geometry;
            return Intersects(g1, g2);
        }

        public bool Intersects(ICanvasItem item1, ICanvasItem item2)
        {
            var geometry1 = GetGeometryInternal(item1);
            geometry1.Transform = ( (ISelectableItem)item1 ).GetTransform().ToMatrixTransform();

            var geometry2 = GetGeometryInternal(item2);
            geometry2.Transform = ( (ISelectableItem)item2 ).GetTransform().ToMatrixTransform();

            return Intersects(geometry1, geometry2);
        }

        public bool Intersects(Geometry g, ICanvasItem item)
        {

            var geometry2 = GetGeometryInternal(item);
            if (geometry2.IsEmpty())
                return false;

            geometry2.Transform = ( (ISelectableItem)item ).GetTransform().ToMatrixTransform();

            return Intersects(g, geometry2);
        }




        public List<XPoint> GetIntersectionPoints(Geometry g1, Geometry g2)
        {
            var points = new List<XPoint>();
            if (g1.Bounds.IntersectsWith(g2.Bounds))
            {
                var cg = GetIntersectionInternal(g1, g2);
                var pg = cg.GetFlattenedPathGeometry();

                // Point[] result = new Point[pg.Figures.Count];
                for (int i = 0; i < pg.Figures.Count; i++)
                {
                    var f = pg.Figures[i];
                    points.AddRange(ToPolyLine(f));
                }
            }
            return points;
        }

        public XPoint[] ToPolyLine(PathFigure figure)
        {
            var outline = new List<XPoint> { figure.StartPoint.ToXPoint() };
            var previousPoint = figure.StartPoint;
            foreach (var segment in figure.Segments)
            {
                var polyline = segment as PolyLineSegment;
                if (polyline != null)
                {
                    outline.AddRange(polyline.Points.Select(p => p.ToXPoint()));
                    previousPoint = polyline.Points.Last();
                    continue;
                }

                var polybezier = segment as PolyBezierSegment;
                if (polybezier != null)
                {
                    for (int i = -1; i + 3 < polybezier.Points.Count; i += 3)
                    {
                        var p1 = i == -1 ? previousPoint : polybezier.Points[i];
                        outline.AddRange(FlattenBezier(p1.ToXPoint(), polybezier.Points[i + 1].ToXPoint(), polybezier.Points[i + 2].ToXPoint(), polybezier.Points[i + 3].ToXPoint(), 20));
                    }

                    previousPoint = polybezier.Points.Last();
                    continue;
                }

                var lineSegment = segment as LineSegment;
                if (lineSegment != null)
                {
                    outline.Add(lineSegment.Point.ToXPoint());
                    previousPoint = lineSegment.Point;
                    continue;
                }

                var bezierSegment = segment as BezierSegment;
                if (bezierSegment != null)
                {
                    outline.AddRange(FlattenBezier(previousPoint.ToXPoint(), bezierSegment.Point1.ToXPoint(), bezierSegment.Point2.ToXPoint(), bezierSegment.Point3.ToXPoint(), 20));
                    previousPoint = bezierSegment.Point3;
                    continue;
                }

                //todo: arc segment
                //var arcSegment = segment as ArcSegment;
                //if(arcSegment!=null)
                //{
                //   figure.fla
                //}

                throw new NotImplementedException();
            }

            return outline.ToArray();
        }

        static IEnumerable<XPoint> FlattenBezier(XPoint p1, XPoint p2, XPoint p3, XPoint p4, int n)
        {
            // http://tsunami.cis.usouthal.edu/~hain/general/Publications/Bezier/bezier%20cccg04%20paper.pdf
            // http://en.wikipedia.org/wiki/De_Casteljau's_algorithm

            for (int i = 1; i <= n; i++)
            {
                var t = (double)i / n;
                var u = 1 - t;
                yield return new XPoint(
                    ( u * u * u * p1.X ) + ( 3 * t * u * u * p2.X ) + ( 3 * t * t * u * p3.X ) + ( t * t * t * p4.X ),
                    ( u * u * u * p1.Y ) + ( 3 * t * u * u * p2.Y ) + ( 3 * t * t * u * p3.Y ) + ( t * t * t * p4.Y ));
            }
        }

        public List<XPoint> GetOutlinePointsInternal(Geometry geometry)
        {
            var points = new List<XPoint>();
            var og = geometry.GetFlattenedPathGeometry(0.05, ToleranceType.Absolute);

            for (int i = 0; i < og.Figures.Count; i++)
            {
                var f = og.Figures[i];
                points.AddRange(ToPolyLine(f));
            }
            return points;
        }

        public IList<XPoint> GetOutlinePoints(object geometry)
        {
            return GetOutlinePointsInternal((Geometry)geometry);
        }

        public CanvasLocation CheckClearance(ICanvasItem item1, ICanvasItem item2, double clearance)
        {
            var g1 = GetGeometryItem(item1);
            //we substract a small value so that it will not intersect and allow for a fixed clearance
            var g1Min = g1.GetWidenedPathGeometry(new Pen(Brushes.Transparent, clearance), 1e-3, ToleranceType.Absolute);

            var g2 = GetGeometryItem(item2);

            //check min clearance
            if (Intersects(g1Min, g2))
            {
                //  result.Message = $"Minimum clearance violation between {item1} and {item2}";
                var intersection = GetIntersectionInternal(g1Min, g2);
                var bounds = intersection.Bounds;
                var location = new CanvasLocation
                {
                    Geometry = new GeometryWrapper(intersection),
                    Location = bounds.ToXRect()// new XRect(bounds.X, bounds.Y, bounds.Width, bounds.Height)
                };
                return location;
            }


            return null;
        }

        protected Geometry GetGeometryItem(ICanvasItem item)
        {
            if (item is PolygonBoardCanvasItem poly)
            {
                var pg = poly.PolygonGeometry.Geometry as Geometry;
                if (pg == null || pg.IsEmpty())
                    return Geometry.Empty;
                var g = pg.Clone();

                var t = poly.GetTransform() as XTransformGroup;

                if (t != null)
                {
                    //poly.PolygonGeometry is in DPI
                    var s = 1.0d / MilimetersToDpiHelper.MillimetersToDpiTransformFactor;
                    t.Children.Add(new XScaleTransform(s, s));
                }
                g.Transform = ToMatrixTransform(t);

                return g;
            }

            var itemGeometry = (Geometry)GetGeometry(item, applyTransform: true);
            // itemGeometry.Transform = ((ISelectableItem)item).GetTransform().ToMatrixTransform();
            return itemGeometry;
        }

        Transform ToMatrixTransform(XTransform t)
        {
            var m = t.Value;
            return new MatrixTransform(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }
    }
}
