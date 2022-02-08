using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Windows.Media;

namespace IDE.Core.Designers
{
    public class PolygonGeometryPourProcessor : BaseGeometryPourProcessor, IPolygonGeometryPourProcessor
    {

        protected override Geometry GetGeometry(ISelectableItem item)
        {
            var thisPoly = item as IPolygonBoardCanvasItem;

            if (thisPoly == null)
                return Geometry.Empty;

            var polygonPoints = thisPoly.PolygonPoints;

            if (polygonPoints.Count == 0)
                return null;

            var segments = new List<LineSegment>();
            var startpoint = polygonPoints[0];
            for (int i = 1; i < polygonPoints.Count; i++)
            {
                segments.Add(new LineSegment(polygonPoints[i].ToPoint(), true));
            }

            var poly = new PathGeometry();
            var figure = new PathFigure(startpoint.ToPoint(), segments, true);
            poly.Figures.Add(figure);

            return poly;
        }


    }
}
