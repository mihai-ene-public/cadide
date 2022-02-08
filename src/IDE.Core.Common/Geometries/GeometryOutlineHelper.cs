using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.Core.Common.Geometries
{
    public class GeometryOutlineHelper : IGeometryOutlineHelper
    {
        ///<inheritdoc/>
        public IGeometryOutline GetGeometry(ICanvasItem item, bool applyTransform = false, double clearance = 0)
        {
            var g = GetGeometryInternal(item, clearance);
            if (applyTransform && item is ISelectableItem s)
            {
                g.Transform = s.GetTransform();
            }

            return g;
        }

        internal IGeometryOutline GetGeometryInternal(ICanvasItem item, double clearance = 0)
        {
            if (item != null)
            {
                switch (item)
                {
                    case ILineCanvasItem line:
                        return GetLineGeometry(line, clearance);

                    case IPolylineCanvasItem polyline:
                        return GetPolylineGeometry(polyline, clearance);

                    case IRectangleCanvasItem rectangle:
                        return GetRectangleGeometry(rectangle, clearance);

                    case IJunctionCanvasItem junction:
                        return GetEllipseGeometry(junction, clearance);

                    case ICircleCanvasItem circle:
                        return GetEllipseGeometry(circle, clearance);

                    case IHoleCanvasItem hole:
                        return GetHoleGeometry(hole, clearance);

                    case IViaCanvasItem via:
                        return GetEllipseGeometry(via, clearance);

                    case IPinCanvasItem pin:
                        return GetPinGeometry(pin);

                    case ILabelCanvasItem label:
                        return GetRectangleGeometry(label);

                    case IPadCanvasItem pad:
                        return GetPadGeometry(pad, clearance);

                    case IPolygonCanvasItem poly:
                        return GetPolygonGeometry(poly, clearance);

                    case IArcCanvasItem arc:
                        return GetArcGeometry(arc);

                    case ITextCanvasItem text:
                        return GetTextGeometry(text);

                    case ITextMonoLineCanvasItem textMono:
                        return GetTextGeometry(textMono);

                    case IRegionCanvasItem region:
                        return GetRegionGeometry(region);

                    case IPlaneBoardCanvasItem plane:
                        return GetPlaneGeometry(plane);
                }
            }

            return null;
        }

        private IGeometryOutline GetLineGeometry(ILineCanvasItem line, double clearance = 0)
        {
            var points = new List<XPoint>();
            points.Add(new XPoint(line.X1, line.Y1));
            points.Add(new XPoint(line.X2, line.Y2));
            return new PolylineGeometryOutline(points, line.Width + 2 * clearance);
        }

        private IGeometryOutline GetPolylineGeometry(IPolylineCanvasItem polyline, double clearance = 0)
        {
            return new PolylineGeometryOutline(polyline.Points, polyline.Width + 2 * clearance);
        }

        private IGeometryOutline GetRectangleGeometry(IRectangleCanvasItem rectangle, double clearance = 0)
        {
            var cornerRadius = 0.0d;
            if (rectangle.CornerRadius > 0.0d)
                cornerRadius = rectangle.CornerRadius + 2 * clearance;

            if (rectangle.IsFilled)
            {
                var width = rectangle.Width + 0.5 * rectangle.BorderWidth;
                var height = rectangle.Height + 0.5 * rectangle.BorderWidth;
                return new RectangleGeometryOutline(0, 0, width + 2 * clearance, height + 2 * clearance, cornerRadius);
            }

            var outterPath = new RectangleGeometryOutline(0, 0, rectangle.Width + 0.5 * rectangle.BorderWidth + 2 * clearance, rectangle.Height + 0.5 * rectangle.BorderWidth + 2 * clearance, cornerRadius);
            var innerPath = new RectangleGeometryOutline(0, 0, rectangle.Width - 0.5 * rectangle.BorderWidth + 2 * clearance, rectangle.Height - 0.5 * rectangle.BorderWidth + 2 * clearance, cornerRadius);

            var outlines = new GeometryOutlines();
            outlines.Outlines.Add(outterPath);

            var innerOutline = innerPath.GetOutline();
            innerOutline.Reverse();
            var poly = new PolygonGeometryOutline(innerOutline);
            outlines.Outlines.Add(poly);

            return outlines;
        }

        private IGeometryOutline GetHoleGeometry(IHoleCanvasItem hole, double clearance = 0)
        {
            if (hole.DrillType == DrillType.Drill)
                return new EllipseGeometryOutline(0, 0, 0.5 * hole.Drill + 2 * clearance);

            //slot
            var t = new XTransformGroup();
            t.Children.Add(new XRotateTransform(hole.Rot));
            t.Children.Add(new XTranslateTransform(hole.X, hole.Y));

            return new RectangleGeometryOutline(0, 0, hole.Drill + 2 * clearance, hole.Height + 2 * clearance, 0.5 * hole.Drill + 2 * clearance);
        }

        private IGeometryOutline GetEllipseGeometry(IJunctionCanvasItem junction, double clearance = 0)
        {
            return new EllipseGeometryOutline(0, 0, 0.5 * junction.Diameter + 2 * clearance);
        }

        private IGeometryOutline GetEllipseGeometry(ICircleCanvasItem circle, double clearance = 0)
        {
            if (circle.IsFilled)
            {
                return new EllipseGeometryOutline(0, 0, 0.5 * circle.Diameter + 0.5 * circle.BorderWidth + 2 * clearance);
            }

            var outterPath = new EllipseGeometryOutline(0, 0, 0.5 * circle.Diameter + 0.5 * circle.BorderWidth + 2 * clearance);
            var innerPath = new EllipseGeometryOutline(0, 0, 0.5 * circle.Diameter - 0.5 * circle.BorderWidth + 2 * clearance);

            var outlines = new GeometryOutlines();
            outlines.Outlines.Add(outterPath);

            var innerOutline = innerPath.GetOutline();
            innerOutline.Reverse();
            var poly = new PolygonGeometryOutline(innerOutline);
            outlines.Outlines.Add(poly);

            return outlines;


        }

        private IGeometryOutline GetEllipseGeometry(IViaCanvasItem via, double clearance = 0)
        {
            return new EllipseGeometryOutline(0, 0, 0.5 * via.Diameter + clearance);
        }

        private IGeometryOutline GetPinGeometry(IPinCanvasItem pin)
        {
            //the geometry will be oriented by the transform which takes into account the orientation

            var startPoint = new XPoint(pin.X, pin.Y);
            var endPoint = new XPoint(startPoint.X + pin.PinLength, startPoint.Y);
            var points = new List<XPoint>
            {
                startPoint,
                endPoint
            };

            return new PolylineGeometryOutline(points, pin.Width);
        }

        private IGeometryOutline GetRectangleGeometry(ILabelCanvasItem label)
        {
            // small rectangle arround the mouse (this it what has to be clicked on a net wire)
            return new RectangleGeometryOutline(label.X, label.Y, 10, 10);
        }

        private IGeometryOutline GetPadGeometry(IPadCanvasItem pad, double clearance = 0)
        {
            var cornerRadius = 0.0d;
            if (pad.CornerRadius > 0.0d)
                cornerRadius = pad.CornerRadius + clearance;

            return new RectangleGeometryOutline(0, 0, pad.Width + 2 * clearance, pad.Height + 2 * clearance, cornerRadius);

        }

        private IGeometryOutline GetPolygonGeometry(IPolygonCanvasItem poly, double clearance = 0)
        {
            return new PolygonGeometryOutline(poly.PolygonPoints);
        }

        private IGeometryOutline GetArcGeometry(IArcCanvasItem arc)
        {
            var sp = new XPoint(arc.StartPointX, arc.StartPointY);
            var ep = new XPoint(arc.EndPointX, arc.EndPointY);
            return new ArcGeometryOutline(sp, ep, arc.Size.Width, arc.Size.Height, arc.SweepDirection, arc.BorderWidth);
        }

        private IGeometryOutline GetTextGeometry(ITextCanvasItem text)
        {
            var fontStyle = FontStyle.Regular;
            if (text.Italic)
                fontStyle = FontStyle.Italic;
            if (text.Bold)
                fontStyle |= FontStyle.Bold;

            var fontSizeMM = text.FontSize;
            //var fontSizeMM =  0.0254 * 1000 * text.FontSize / 96.0d;
            var scale = 0.0254 * 1000 / 96.0d;

            var font = SystemFonts.CreateFont(text.FontFamily, (float)fontSizeMM, fontStyle);
            var style = new RendererOptions(font, 72.0f);
            style.ApplyKerning = false;
            var paths = TextBuilder.GenerateGlyphs(text.Text, style);

            var geometries = new GeometryOutlines();
            foreach (var path in paths)
            {
                var innerPaths = path.Flatten();

                foreach (var innerPath in innerPaths)
                {
                    var points = innerPath.Points.Select(p => new XPoint(p.X * scale, p.Y * scale)).ToList();

                    var g = new PolygonGeometryOutline(points);
                    geometries.Outlines.Add(g);
                }
            }

            return geometries;
        }

        private IGeometryOutline GetTextGeometry(ITextMonoLineCanvasItem text)
        {
            var geometries = new GeometryOutlines();

            var x = 0.0d;
            foreach (var letter in text.LetterItems)
            {

                foreach (var item in letter.Items)
                {
                    var gItem = GetGeometryInternal(item);
                    var lt = new XTransformGroup();

                    lt.Children.Add(new XTranslateTransform(x, 0));

                    gItem.Transform = lt;

                    geometries.Outlines.Add(gItem);
                }

                x += letter.FontSize;
            }

            return geometries;
        }

        private IGeometryOutline GetRegionGeometry(IRegionCanvasItem region)
        {
            var pathGeometry = new PathGeometryOutline(region.StartPoint);

            foreach (var item in region.Items)
            {
                switch (item)
                {
                    case ILineRegionItem line:
                        pathGeometry.AddLine(line.EndPoint);
                        break;

                    case IArcRegionItem arc:
                        pathGeometry.AddArc(arc.EndPoint, arc.SizeDiameter, arc.SweepDirection);
                        break;
                }

            }

            return pathGeometry;
        }

        private IGeometryOutline GetPlaneGeometry(IPlaneBoardCanvasItem plane)
        {
            try
            {
                dynamic brdItem = plane;
                var brd = brdItem.LayerDocument as IBoardDesigner;
                if (brd != null)
                {
                    return GetRegionGeometry(brd.BoardOutline);
                }
            }
            catch
            {
            }
            return null;
        }
    }
}
