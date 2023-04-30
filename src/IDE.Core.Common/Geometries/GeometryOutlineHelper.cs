using IDE.Core.Errors;
using IDE.Core.Interfaces;
using IDE.Core.Interfaces.Geometries;
using IDE.Core.Types.Media;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IDE.Core.Common.Geometries;

public class GeometryOutlineHelper : IGeometryOutlineHelper
{
    ///<inheritdoc/>
    public IGeometryOutline GetGeometry(ICanvasItem item, bool applyTransform = false, double clearance = 0)
    {
        var g = GetGeometryInternal(item, clearance);
        if (applyTransform && g != null && item is ISelectableItem s)
        {
            g.Transform = s.GetTransform();
        }

        return g;
    }

    public IGeometryOutline GetShapeGeometry(IShape item)
    {
        var g = GetGeometryInternal(item);
        return g;
    }

    public bool Intersects(ICanvasItem item1, ICanvasItem item2)
    {
        var geometry1 = GetGeometryInternal(item1);
        geometry1.Transform = ((ISelectableItem)item1).GetTransform();

        var geometry2 = GetGeometryInternal(item2);
        geometry2.Transform = ((ISelectableItem)item2).GetTransform();

        return GeometryOutline.Intersects(geometry1, geometry2);
    }

    public bool ItemIntersectsRectangle(ICanvasItem item, XRect rect)
    {
        var itemBounds = ((ISelectableItem)item).GetBoundingRectangle();

        if (rect.IntersectsWith(itemBounds))
        {
            var itemGeometry = GetGeometryInternal(item);
            if (itemGeometry is null)
                return false;

            itemGeometry.Transform = ((ISelectableItem)item).GetTransform();

            var center = rect.GetCenter();
            var rectGeom = new RectangleGeometryOutline(center.X, center.Y, rect.Width, rect.Height);

            return GeometryOutline.Intersects(rectGeom, itemGeometry);
        }

        return false;
    }

    public CanvasLocation CheckClearance(ICanvasItem item1, ICanvasItem item2, double clearance)
    {
        var geometry1 = GetGeometryInternal(item1);
        geometry1.Transform = ((ISelectableItem)item1).GetTransform();

        var geometry2 = GetGeometryInternal(item2);
        geometry2.Transform = ((ISelectableItem)item2).GetTransform();

        if (GeometryOutline.Intersects(geometry1, geometry2))
        {
            var intersection = GeometryOutline.Combine(
                new List<IGeometryOutline> { geometry1 },
                new List<IGeometryOutline> { geometry2 },
                GeometryCombineMode.Intersect);

            var bounds = intersection.GetBounds();
            var location = new CanvasLocation
            {
                Geometry = intersection,
                Location = bounds
            };
            return location;
        }

        return null;
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

    internal IGeometryOutline GetGeometryInternal(IShape item)
    {
        switch (item)
        {
            case ILineShape line:
                return GetLineGeometry(line);

            case IPolylineShape polyline:
                return GetPolylineGeometry(polyline);

            case IRectangleShape rectangle:
                return GetRectangleGeometry(rectangle);

            case ICircleShape circle:
                return GetEllipseGeometry(circle);

            case IHoleShape hole:
                return GetHoleGeometry(hole);

            case IViaShape via:
                return GetEllipseGeometry(via);

            case IPolygonShape poly:
                return GetPolygonGeometry(poly);

            case IPouredPolygonShape pouredPolygon:
                return GetPouredPolygonGeometry(pouredPolygon);

            case IArcShape arc:
                return GetArcGeometry(arc);

            case ITextShape text:
                return GetTextGeometry(text);

            case IFigureShape fig:
                return GetFigureGeometry(fig);

            case IRegionShape region:
                return GetRegionGeometry(region);

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

    private IGeometryOutline GetLineGeometry(ILineShape line)
    {
        var points = new List<XPoint>();
        points.Add(line.StartPoint);
        points.Add(line.EndPoint);
        return new PolylineGeometryOutline(points, line.Width);
    }

    private IGeometryOutline GetPolylineGeometry(IPolylineCanvasItem polyline, double clearance = 0)
    {
        return new PolylineGeometryOutline(polyline.Points, polyline.Width + 2 * clearance);
    }

    private IGeometryOutline GetPolylineGeometry(IPolylineShape polyline)
    {
        return new PolylineGeometryOutline(polyline.Points, polyline.Width);
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

    private IGeometryOutline GetRectangleGeometry(IRectangleShape rectangle)
    {
        //var cornerRadius = 0.0d;
        //if (rectangle.CornerRadius > 0.0d)
        //    cornerRadius = rectangle.CornerRadius;

        //if (rectangle.IsFilled)
        //{
        //    var width = rectangle.Width + 0.5 * rectangle.BorderWidth;
        //    var height = rectangle.Height + 0.5 * rectangle.BorderWidth;
        //    return new RectangleGeometryOutline(0, 0, width, height, cornerRadius);
        //}

        //var outterPath = new RectangleGeometryOutline(0, 0, rectangle.Width + 0.5 * rectangle.BorderWidth, rectangle.Height + 0.5 * rectangle.BorderWidth, cornerRadius);
        //var innerPath = new RectangleGeometryOutline(0, 0, rectangle.Width - 0.5 * rectangle.BorderWidth, rectangle.Height - 0.5 * rectangle.BorderWidth, cornerRadius);

        //var outlines = new GeometryOutlines();
        //outlines.Outlines.Add(outterPath);

        //var innerOutline = innerPath.GetOutline();
        //innerOutline.Reverse();
        //var poly = new PolygonGeometryOutline(innerOutline);
        //outlines.Outlines.Add(poly);

        //return outlines;

        var transform = new XTransformGroup();
        transform.Children.Add(new XRotateTransform(rectangle.Rot));
        transform.Children.Add(new XTranslateTransform(rectangle.X, rectangle.Y));

        var cornerRadius = 0.0d;
        if (rectangle.CornerRadius > 0.0d)
            cornerRadius = rectangle.CornerRadius;
        var outline = new RectangleGeometryOutline(0, 0, rectangle.Width, rectangle.Height, cornerRadius);
        outline.Transform = transform;
        return outline;
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

    private IGeometryOutline GetHoleGeometry(IHoleShape hole)
    {
        return new EllipseGeometryOutline(hole.X, hole.Y, 0.5 * hole.Drill);
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
        if (circle.BorderWidth == 0.00d)
        {
            return new EllipseGeometryOutline(0, 0, 0.5 * circle.Diameter + 2 * clearance);
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

    private IGeometryOutline GetEllipseGeometry(ICircleShape circle)
    {
        if (circle.IsFilled)
        {
            return new EllipseGeometryOutline(circle.X, circle.Y, 0.5 * circle.Diameter + 0.5 * circle.BorderWidth);
        }

        var outterPath = new EllipseGeometryOutline(circle.X, circle.Y, 0.5 * circle.Diameter + 0.5 * circle.BorderWidth);
        var innerPath = new EllipseGeometryOutline(circle.X, circle.Y, 0.5 * circle.Diameter - 0.5 * circle.BorderWidth);

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

    private IGeometryOutline GetEllipseGeometry(IViaShape via)
    {
        return new EllipseGeometryOutline(via.X, via.Y, 0.5 * via.PadDiameter);
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

    private IGeometryOutline GetPolygonGeometry(IPolygonShape poly)
    {
        return new PolygonGeometryOutline(poly.Points);
    }

    private IGeometryOutline GetPouredPolygonGeometry(IPouredPolygonShape poly)
    {
        return poly.FinalGeometry;
    }

    private IGeometryOutline GetFigureGeometry(IFigureShape fig)
    {
        var geometries = new GeometryOutlines();

        foreach (var g in fig.FigureShapes)
        {
            var outline = GetShapeGeometry(g);
            if (outline != null)
            {
                geometries.Outlines.Add(outline);
            }
        }

        return geometries;
    }

    private IGeometryOutline GetArcGeometry(IArcCanvasItem arc)
    {
        var sp = new XPoint(arc.StartPointX, arc.StartPointY);
        var ep = new XPoint(arc.EndPointX, arc.EndPointY);
        return new ArcGeometryOutline(sp, ep, arc.Size.Width, arc.Size.Height, arc.SweepDirection, arc.BorderWidth);
    }

    private IGeometryOutline GetArcGeometry(IArcShape arc)
    {
        return new ArcGeometryOutline(arc.StartPoint, arc.EndPoint, 0.5 * arc.SizeDiameter, 0.5 * arc.SizeDiameter, arc.SweepDirection, arc.Width);
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

    private IGeometryOutline GetTextGeometry(ITextShape text)
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

        var t = new XTransformGroup();
        t.Children.Add(new XRotateTransform(text.Rot));
        t.Children.Add(new XTranslateTransform(text.X, text.Y));

        geometries.Transform = t;

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

    private IGeometryOutline GetRegionGeometry(IRegionShape region)
    {
        var pathGeometry = new PathGeometryOutline(region.StartPoint);

        foreach (var item in region.Items)
        {
            switch (item)
            {
                case ILineShape line:
                    pathGeometry.AddLine(line.EndPoint);
                    break;

                case IArcShape arc:
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
